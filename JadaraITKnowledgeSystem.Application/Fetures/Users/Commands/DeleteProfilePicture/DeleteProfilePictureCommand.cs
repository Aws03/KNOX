using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Fetures.Users.Commands.DeleteProfilePicture;

public sealed record DeleteProfilePictureCommand() : IRequest<Result<Success>>;
