using JadaraITKnowledgeSystem.Application.Fetures.Users.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Fetures.Users.Queries.GetCurrentUserProfile
{
    public sealed record GetCurrentUserProfileQuery() : IRequest<Result<UserProfileDto>>;
}
