using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using JadaraITKnowledgeSystem.Infrastructure.Identity;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using MediatR;
using JadaraITKnowledgeSystem.Application.Fetures.Users.Commands.CreateUser;
using JadaraITKnowledgeSystem.Application.Fetures.Users.Commands.VerifyAccount;
using JadaraITKnowledgeSystem.Application.Fetures.Users.Commands.ResetPassword;
using Microsoft.Extensions.Logging;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Application.Fetures.Users.Queries.GetUserVerificationStatus;

namespace JadaraITKnowledgeSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenService _tokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;
    private readonly IOTPService _otpService;
    private readonly IConfiguration _configuration;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenService tokenService,
        IRefreshTokenService refreshTokenService,
        IMediator mediator,
        ILogger<AuthController> logger,
        IOTPService otpService,
        IConfiguration configuration
    )
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _refreshTokenService = refreshTokenService;
        _mediator = mediator;
        _logger = logger;
        _otpService = otpService;
        _configuration = configuration;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[Register] Attempting registration for {Email}", request.Email);

        var command = new CreateUserCommand(
            FullName: request.FullName,
            Email: request.Email,
            MajorId: request.MajorId,
            Password: request.Password
        );

        var result = await _mediator.Send(command, cancellationToken);

        return await result.Match<Task<IActionResult>>(
            onValue: async dto =>
            {
                _logger.LogInformation("[Register] User registered successfully: {Email}", request.Email);

                var requireEmailVerification = _configuration.GetValue<bool>("AuthSettings:RequireEmailVerificationOnRegisteration");

                if (requireEmailVerification)
                {
                    // Send OTP for email verification
                    var otpSent = await _otpService.SendOtpAsync(request.Email);
                    if (!otpSent)
                    {
                        _logger.LogWarning("[Register] Failed to send OTP to {Email}", request.Email);
                        return Ok(new
                        {
                            user = dto,
                            requiresVerification = true,
                            message = "Registration successful. Failed to send OTP, please request a new one.",
                            otpSent = false
                        });
                    }

                    return Ok(new
                    {
                        user = dto,
                        requiresVerification = true,
                        message = "Registration successful. Please verify your email using the OTP sent.",
                        otpSent = true
                    });
                }
                else
                {
                    // No verification required - return tokens immediately
                    var identityUser = await _userManager.FindByEmailAsync(request.Email);
                    if (identityUser == null)
                    {
                        _logger.LogError("[Register] Identity user not found after registration: {Email}", request.Email);
                        return StatusCode(500, new { message = "User created but token generation failed" });
                    }

                    var roles = await _userManager.GetRolesAsync(identityUser);
                    var accessToken = await _tokenService.GenerateJwtTokenAsync(
                        identityUser.Id,
                        identityUser.FullName,
                        identityUser.Email,
                        roles
                    );

                    var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    var refreshTokenResult = await _refreshTokenService.GenerateRefreshTokenAsync(identityUser.Id, ipAddress);

                    if (!refreshTokenResult.IsSuccess)
                    {
                        _logger.LogError("[Register] Refresh token generation failed for {Email}", request.Email);
                        return StatusCode(500, new { message = "Token generation failed" });
                    }

                    return Ok(new
                    {
                        user = dto,
                        requiresVerification = false,
                        message = "Registration successful",
                        accessToken,
                        refreshToken = refreshTokenResult.Value.Token,
                        expiresAt = refreshTokenResult.Value.ExpiresAt
                    });
                }
            },
            onError: errors =>
            {
                _logger.LogWarning("[Register] Registration failed for {Email}: {Errors}",
                    request.Email, string.Join("; ", errors.Select(e => e.Description)));
                return Task.FromResult<IActionResult>(BadRequest(new { errors }));
            }
        );
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("[Login] Login attempt for {Email}", request.Email);

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            _logger.LogWarning("[Login] User not found: {Email}", request.Email);
            return Unauthorized(new { message = "Invalid credentials" });
        }

        // Enforce verification if configured
        var requireVerification = _configuration.GetValue<bool>("AuthSettings:RequireEmailVerificationOnRegisteration");
        if (requireVerification)
        {
            var verificationResult = await _mediator.Send(new GetUserVerificationStatusQuery(request.Email));
            if (verificationResult.IsError)
            {
                _logger.LogWarning("[Login] Verification status check failed for {Email}", request.Email);
                return Unauthorized(new { message = "Email not verified. Please verify your email to continue." });
            }

            if (!verificationResult.Value)
            {
                _logger.LogWarning("[Login] User not verified: {Email}", request.Email);
                return Unauthorized(new { message = "Email not verified. Please verify your email to continue." });
            }
        }

        var check = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (!check.Succeeded)
        {
            _logger.LogWarning("[Login] Password check failed for {Email}", request.Email);
            return Unauthorized(new { message = "Invalid credentials" });
        }

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = await _tokenService.GenerateJwtTokenAsync(user.Id, user.FullName, user.Email, roles);

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var refreshTokenResult = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id, ipAddress);

        if (!refreshTokenResult.IsSuccess)
        {
            _logger.LogError("[Login] Refresh token generation failed for {Email}", request.Email);
            return StatusCode(500, new { message = "Token generation failed" });
        }

        _logger.LogInformation("[Login] Login successful for {Email}", request.Email);

        return Ok(new
        {
            accessToken,
            refreshToken = refreshTokenResult.Value.Token,
            expiresAt = refreshTokenResult.Value.ExpiresAt
        });
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        _logger.LogInformation("[RefreshToken] Token refresh attempt");

        var tokenResult = await _refreshTokenService.GetActiveRefreshTokenAsync(request.RefreshToken);
        if (!tokenResult.IsSuccess)
        {
            _logger.LogWarning("[RefreshToken] Invalid or expired refresh token");
            return Unauthorized(new { message = "Invalid or expired refresh token" });
        }

        var oldToken = tokenResult.Value;
        var user = await _userManager.FindByIdAsync(oldToken.UserId.ToString());
        if (user is null)
        {
            _logger.LogWarning("[RefreshToken] User not found for UserId={UserId}", oldToken.UserId);
            return Unauthorized(new { message = "User not found" });
        }

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        await _refreshTokenService.RevokeTokenAsync(oldToken.Token, ipAddress);

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = await _tokenService.GenerateJwtTokenAsync(user.Id, user.FullName, user.Email, roles);
        var newRefreshTokenResult = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id, ipAddress);

        if (!newRefreshTokenResult.IsSuccess)
        {
            _logger.LogError("[RefreshToken] New refresh token generation failed for UserId={UserId}", user.Id);
            return StatusCode(500, new { message = "Token generation failed" });
        }

        _logger.LogInformation("[RefreshToken] Token refreshed successfully for UserId={UserId}", user.Id);

        return Ok(new
        {
            accessToken,
            refreshToken = newRefreshTokenResult.Value.Token,
            expiresAt = newRefreshTokenResult.Value.ExpiresAt
        });
    }

    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("[Logout] Invalid user claim");
            return BadRequest(new { message = "Invalid user" });
        }

        _logger.LogInformation("[Logout] Logout for UserId={UserId}", userId);

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        if (!string.IsNullOrEmpty(request.RefreshToken))
        {
            await _refreshTokenService.RevokeTokenAsync(request.RefreshToken, ipAddress);
        }
        else
        {
            await _refreshTokenService.RevokeAllUserTokensAsync(userId, ipAddress);
        }

        _logger.LogInformation("[Logout] Logout successful for UserId={UserId}", userId);
        return Ok(new { message = "Logout successful" });
    }

    [HttpPost("assign-role")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
    {
        _logger.LogInformation("[AssignRole] Assigning role {Role} to UserId={UserId}", request.Role, request.UserId);

        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
        {
            _logger.LogWarning("[AssignRole] User not found: UserId={UserId}", request.UserId);
            return NotFound(new { message = "User not found" });
        }

        var roleExists = await _userManager.GetRolesAsync(user);
        if (roleExists.Contains(request.Role))
        {
            _logger.LogWarning("[AssignRole] User already has role {Role}", request.Role);
            return BadRequest(new { message = "User already has this role" });
        }

        var result = await _userManager.AddToRoleAsync(user, request.Role);
        if (!result.Succeeded)
        {
            _logger.LogWarning("[AssignRole] Failed to assign role {Role} to UserId={UserId}: {Errors}", request.Role, request.UserId, string.Join("; ", result.Errors.Select(e => e.Description)));
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        }

        _logger.LogInformation("[AssignRole] Role {Role} assigned successfully to UserId={UserId}", request.Role, request.UserId);
        return Ok(new { message = $"Role '{request.Role}' assigned successfully" });
    }

    [HttpPost("send-verification-otp")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> SendVerificationOtp([FromBody] SendOtpRequest request)
    {
        _logger.LogInformation("[SendVerificationOtp] Sending OTP to {Email}", request.Email);

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            _logger.LogWarning("[SendVerificationOtp] Email is required");
            return BadRequest(new { message = "Email is required" });
        }

        var sent = await _otpService.SendOtpAsync(request.Email);
        if (!sent)
        {
            _logger.LogWarning("[SendVerificationOtp] Failed to send OTP to {Email}", request.Email);
            return StatusCode(StatusCodes.Status429TooManyRequests, 
                new { message = "Please wait before requesting another OTP or user not found" });
        }

        _logger.LogInformation("[SendVerificationOtp] OTP sent successfully to {Email}", request.Email);
        return Ok(new { message = "OTP sent successfully to your email" });
    }

    [HttpPost("verify-account")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> VerifyAccount([FromBody] VerifyOtpRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[VerifyAccount] Verifying account for {Email}", request.Email);

        var command = new VerifyAccountCommand(request.Email, request.Otp);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsError)
        {
            _logger.LogWarning("[VerifyAccount] Verification failed for {Email}: {Errors}", 
                request.Email, string.Join("; ", result.Errors.Select(e => e.Description)));
            return BadRequest(new { message = result.TopError.Description });
        }

        // Generate tokens after successful verification
        var identityUser = await _userManager.FindByEmailAsync(request.Email);
        if (identityUser == null)
        {
            _logger.LogError("[VerifyAccount] Identity user not found after verification: {Email}", request.Email);
            return StatusCode(500, new { message = "Verification successful but token generation failed" });
        }

        var roles = await _userManager.GetRolesAsync(identityUser);
        var accessToken = await _tokenService.GenerateJwtTokenAsync(
            identityUser.Id,
            identityUser.FullName,
            identityUser.Email,
            roles
        );

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var refreshTokenResult = await _refreshTokenService.GenerateRefreshTokenAsync(identityUser.Id, ipAddress);

        if (!refreshTokenResult.IsSuccess)
        {
            _logger.LogError("[VerifyAccount] Refresh token generation failed for {Email}", request.Email);
            return StatusCode(500, new { message = "Verification successful but token generation failed" });
        }

        _logger.LogInformation("[VerifyAccount] Account verified successfully for {Email}", request.Email);
        return Ok(new
        {
            message = "Account verified successfully",
            accessToken,
            refreshToken = refreshTokenResult.Value.Token,
            expiresAt = refreshTokenResult.Value.ExpiresAt
        });
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[ResetPassword] Password reset attempt for {Email}", request.Email);

        var command = new ResetPasswordCommand(request.Email, request.Otp, request.NewPassword);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsError)
        {
            _logger.LogWarning("[ResetPassword] Password reset failed for {Email}: {Errors}",
                request.Email, string.Join("; ", result.Errors.Select(e => e.Description)));
            return BadRequest(new { message = result.TopError.Description });
        }

        _logger.LogInformation("[ResetPassword] Password reset successfully for {Email}", request.Email);
        return Ok(new { message = "Password reset successfully" });
    }

    public sealed record RegisterRequest(string Email, string Password, string FullName, int MajorId, string? Role);
    public sealed record LoginRequest(string Email, string Password);
    public sealed record RefreshTokenRequest(string RefreshToken);
    public sealed record LogoutRequest(string? RefreshToken);
    public sealed record AssignRoleRequest(int UserId, string Role);
    public sealed record SendOtpRequest(string Email);
    public sealed record VerifyOtpRequest(string Email, string Otp);
    public sealed record ResetPasswordRequest(string Email, string Otp, string NewPassword);
}
