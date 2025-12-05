using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Fetures.Users.Commands.CreateUser
{
    public sealed record CreateUserCommand(
        string FullName,
        string Email,
        int MajorId,
        string? Password
    ) : IRequest<Result<CreateUserResultDto>>;

    public sealed record CreateUserResultDto(
        int DomainUserId,
        int IdentityUserId,
        string Email,
        string AssignedRole
    );
}
