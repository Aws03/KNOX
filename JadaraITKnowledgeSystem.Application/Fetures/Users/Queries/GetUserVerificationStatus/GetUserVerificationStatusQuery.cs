using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Fetures.Users.Queries.GetUserVerificationStatus;

public sealed record GetUserVerificationStatusQuery(string Email) : IRequest<Result<bool>>;
