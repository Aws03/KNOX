using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Users.Commands.DeleteProfilePicture;

public sealed record DeleteProfilePictureCommand() : IRequest<Result<Success>>;
