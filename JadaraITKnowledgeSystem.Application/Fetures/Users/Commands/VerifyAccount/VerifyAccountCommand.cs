using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Fetures.Users.Commands.VerifyAccount;

public sealed record VerifyAccountCommand(string Email, string Otp) : IRequest<Result<Success>>;
