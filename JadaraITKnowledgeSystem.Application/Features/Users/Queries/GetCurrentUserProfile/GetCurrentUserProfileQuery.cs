using JadaraITKnowledgeSystem.Application.Features.Users.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Users.Queries.GetCurrentUserProfile
{
    public sealed record GetCurrentUserProfileQuery() : IRequest<Result<UserProfileDto>>;
}
