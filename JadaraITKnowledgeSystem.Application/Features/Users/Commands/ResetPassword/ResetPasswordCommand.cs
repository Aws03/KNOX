using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Users.Commands.ResetPassword;

public sealed record ResetPasswordCommand(
    string Email,
    string Otp,
    string NewPassword
) : IRequest<Result<Success>>;
