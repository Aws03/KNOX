using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Users.Queries.GetUserVerificationStatus;

public sealed record GetUserVerificationStatusQuery(string Email) : IRequest<Result<bool>>;
