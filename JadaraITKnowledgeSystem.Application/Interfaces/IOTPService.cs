using JadaraITKnowledgeSystem.Domain.Common.Results;

namespace JadaraITKnowledgeSystem.Application.Interfaces;

public interface IOTPService
{
    Task<string> GenerateOTPAsync(int userId);
    Task<Result<int>> ValidateOtpAsync(string email, string otp);
    Task<bool> SendOtpAsync(string email);
    Task<bool> CheckOTPValidityByEmailAsync(string email, string otp);
}
