using JadaraITKnowledgeSystem.Domain.Common.Results;

namespace JadaraITKnowledgeSystem.Application.Interfaces;

public interface IOTPService
{
    /// <summary>
    /// Emails a fresh code. Returns false when the address is unknown or a code was sent
    /// too recently (callers must not tell the two apart, to avoid account enumeration).
    /// </summary>
    Task<bool> SendOtpAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Validates and consumes the user's latest code; returns the domain user id.</summary>
    Task<Result<int>> ValidateOtpAsync(string email, string otp, CancellationToken cancellationToken = default);
}
