using JadaraITKnowledgeSystem.Application.Fetures.Users.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using System.IO;

namespace JadaraITKnowledgeSystem.Application.Fetures.Users.Commands.UpdateProfilePicture;

public sealed record UpdateProfilePictureCommand(Stream ImageStream, string FileName) : IRequest<Result<UserProfileDto>>;
