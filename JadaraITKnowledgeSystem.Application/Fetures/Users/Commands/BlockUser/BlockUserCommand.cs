using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Fetures.Users.Commands.BlockUser;

public sealed record BlockUserCommand(int UserId) : IRequest<Result<Success>>;
