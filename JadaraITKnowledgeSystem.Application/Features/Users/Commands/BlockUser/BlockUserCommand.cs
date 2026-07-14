using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Users.Commands.BlockUser;

public sealed record BlockUserCommand(int UserId) : IRequest<Result<Success>>;
