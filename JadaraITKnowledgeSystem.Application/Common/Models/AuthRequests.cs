namespace JadaraITKnowledgeSystem.Application.Common.Models;

public sealed record RegisterRequest(string Email, string Password, string FullName, int MajorId, string? Role);
public sealed record LoginRequest(string Email, string Password);
public sealed record RefreshTokenRequest(string RefreshToken);
public sealed record LogoutRequest(string? RefreshToken);
public sealed record AssignRoleRequest(int UserId, string Role);
public sealed record SendOtpRequest(string Email);
public sealed record VerifyOtpRequest(string Email, string Otp);
public sealed record ResetPasswordRequest(string Email, string Otp, string NewPassword);
public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);
