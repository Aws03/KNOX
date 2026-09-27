namespace JadaraITKnowledgeSystem.API.Extensions;

public static class RateLimitPolicies
{
    /// <summary>Login/registration: 5 requests per minute per client IP.</summary>
    public const string Auth = "AuthPolicy";

    /// <summary>OTP send/verify and password reset: 3 requests per minute per client IP.</summary>
    public const string Otp = "OtpPolicy";
}
