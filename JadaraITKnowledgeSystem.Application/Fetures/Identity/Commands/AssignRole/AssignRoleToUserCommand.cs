using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Fetures.Identity.Commands.AssignRole;

public sealed record AssignRoleToUserCommand(int UserId, string RoleName) : IRequest<Result<Success>>;
