using JadaraITKnowledgeSystem.API.Contracts;
using JadaraITKnowledgeSystem.API.Extensions;
using JadaraITKnowledgeSystem.Application.Features.Auth.Commands.Login;
using JadaraITKnowledgeSystem.Application.Features.Auth.Commands.Logout;
using JadaraITKnowledgeSystem.Application.Features.Auth.Commands.RefreshToken;
using JadaraITKnowledgeSystem.Application.Features.Auth.Commands.Register;
using JadaraITKnowledgeSystem.Application.Features.Auth.Commands.SendVerificationOtp;
using JadaraITKnowledgeSystem.Application.Features.Auth.Commands.VerifyAccount;
using JadaraITKnowledgeSystem.Application.Features.Auth.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Users.Commands.ChangePassword;
using JadaraITKnowledgeSystem.Application.Features.Users.Commands.ResetPassword;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace JadaraITKnowledgeSystem.API.Controllers;

[Route("api/auth")]
public sealed class AuthController(ISender sender) : ApiControllerBase(sender)
{
    /// <summary>Creates an account. Returns tokens unless email verification is required.</summary>
    [HttpPost("register")]
    [EnableRateLimiting(RateLimitPolicies.Auth)]
    [ProducesResponseType<RegistrationResultDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken) =>
        OkOrProblem(await Sender.Send(
            new RegisterCommand(request.FullName, request.Email, request.MajorId, request.Password, ClientIpAddress),
            cancellationToken));

    [HttpPost("login")]
    [EnableRateLimiting(RateLimitPolicies.Auth)]
    [ProducesResponseType<AuthTokensDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken) =>
        OkOrProblem(await Sender.Send(new LoginCommand(request.Email, request.Password, ClientIpAddress), cancellationToken));

    /// <summary>Exchanges a refresh token for a new token pair (the old refresh token is consumed).</summary>
    [HttpPost("refresh")]
    [ProducesResponseType<AuthTokensDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken) =>
        OkOrProblem(await Sender.Send(new RefreshTokenCommand(request.RefreshToken, ClientIpAddress), cancellationToken));

    /// <summary>Revokes the given refresh token, or all of the caller's refresh tokens.</summary>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest? request, CancellationToken cancellationToken) =>
        NoContentOrProblem(await Sender.Send(new LogoutCommand(request?.RefreshToken, ClientIpAddress), cancellationToken));

    /// <summary>Emails a one-time code for account verification or password reset.</summary>
    [HttpPost("send-verification-otp")]
    [EnableRateLimiting(RateLimitPolicies.Otp)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SendVerificationOtp([FromBody] SendOtpRequest request, CancellationToken cancellationToken) =>
        NoContentOrProblem(await Sender.Send(new SendVerificationOtpCommand(request.Email), cancellationToken));

    [HttpPost("verify-account")]
    [EnableRateLimiting(RateLimitPolicies.Otp)]
    [ProducesResponseType<AuthTokensDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyAccount([FromBody] VerifyOtpRequest request, CancellationToken cancellationToken) =>
        OkOrProblem(await Sender.Send(new VerifyAccountCommand(request.Email, request.Otp, ClientIpAddress), cancellationToken));

    [HttpPost("reset-password")]
    [EnableRateLimiting(RateLimitPolicies.Otp)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken) =>
        NoContentOrProblem(await Sender.Send(new ResetPasswordCommand(request.Email, request.Otp, request.NewPassword), cancellationToken));

    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken) =>
        NoContentOrProblem(await Sender.Send(new ChangePasswordCommand(request.CurrentPassword, request.NewPassword), cancellationToken));
}
