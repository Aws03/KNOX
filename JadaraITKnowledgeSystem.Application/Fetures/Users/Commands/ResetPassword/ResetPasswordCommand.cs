using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Fetures.Users.Commands.ResetPassword;

public sealed record ResetPasswordCommand(
    string Email,
    string Otp,
    string NewPassword
) : IRequest<Result<Success>>;
