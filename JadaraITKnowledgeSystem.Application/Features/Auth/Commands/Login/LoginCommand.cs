using JadaraITKnowledgeSystem.Application.Features.Auth.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(string Email, string Password, string IpAddress) : IRequest<Result<AuthTokensDto>>;
