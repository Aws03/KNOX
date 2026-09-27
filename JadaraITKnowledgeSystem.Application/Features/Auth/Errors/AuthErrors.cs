using JadaraITKnowledgeSystem.Domain.Common.Results;

namespace JadaraITKnowledgeSystem.Application.Features.Auth.Errors;

public static class AuthErrors
{
    // Deliberately identical for "no such user" and "wrong password" so the login
    // endpoint can't be used to probe which emails are registered.
    public static Error InvalidCredentials =>
        Error.Unauthorized("Auth.InvalidCredentials", "Invalid credentials");

    public static Error EmailNotVerified =>
        Error.Unauthorized("Auth.EmailNotVerified", "Email not verified. Please verify your email to continue.");

    public static Error AccountBlocked =>
        Error.Unauthorized("Auth.AccountBlocked", "This account has been blocked. Please contact an administrator.");

    public static Error InvalidRefreshToken =>
        Error.Unauthorized("Auth.InvalidRefreshToken", "Invalid or expired refresh token");

    public static Error UserNotFound =>
        Error.Unauthorized("Auth.UserNotFound", "User not found");
}
