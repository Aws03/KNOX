using JadaraITKnowledgeSystem.Application.Fetures.Users.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace JadaraITKnowledgeSystem.Application.Fetures.Users.Commands.UpdateProfilePicture;

public sealed record UpdateProfilePictureCommand(IFormFile Image) : IRequest<Result<UserProfileDto>>;
