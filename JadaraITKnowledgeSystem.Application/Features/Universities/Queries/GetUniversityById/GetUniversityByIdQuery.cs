using JadaraITKnowledgeSystem.Application.Features.Universities.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Universities.Queries.GetUniversityById;

public sealed record GetUniversityByIdQuery(int UniversityId)
    : IRequest<Result<UniversityDto>>;
