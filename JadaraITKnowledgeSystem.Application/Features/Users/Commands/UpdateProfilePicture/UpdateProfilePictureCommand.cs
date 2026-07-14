using JadaraITKnowledgeSystem.Application.Features.Users.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using System.IO;

namespace JadaraITKnowledgeSystem.Application.Features.Users.Commands.UpdateProfilePicture;

public sealed record UpdateProfilePictureCommand(Stream ImageStream, string FileName) : IRequest<Result<UserProfileDto>>;
