using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Fetures.Users.Commands.ActivateUser;

public sealed record ActivateUserCommand(int UserId) : IRequest<Result<Success>>;
