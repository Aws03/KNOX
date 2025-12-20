using JadaraITKnowledgeSystem.Application.Fetures.Users.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Fetures.Users.Commands.UpdateUserProfile;

public sealed record UpdateUserProfileCommand(
    string? FullName = null,
    int? MajorId = null
) : IRequest<Result<UserProfileDto>>;
