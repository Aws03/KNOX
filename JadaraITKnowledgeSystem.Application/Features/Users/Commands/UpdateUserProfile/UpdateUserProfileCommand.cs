using JadaraITKnowledgeSystem.Application.Features.Users.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Users.Commands.UpdateUserProfile;

public sealed record UpdateUserProfileCommand(
    string? FullName = null,
    int? MajorId = null
) : IRequest<Result<UserProfileDto>>;
