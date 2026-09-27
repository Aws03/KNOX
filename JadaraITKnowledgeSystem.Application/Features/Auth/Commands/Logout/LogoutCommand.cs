using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Auth.Commands.Logout;

/// <summary>
/// Revokes <paramref name="RefreshToken"/> if given, otherwise every refresh token the (authenticated) caller holds.
/// </summary>
public sealed record LogoutCommand(string? RefreshToken, string IpAddress) : IRequest<Result<Success>>;
