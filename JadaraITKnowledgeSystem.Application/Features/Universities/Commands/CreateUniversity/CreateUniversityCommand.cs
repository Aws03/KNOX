using JadaraITKnowledgeSystem.Application.Features.Universities.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Universities.Commands.CreateUniversity;

public sealed record CreateUniversityCommand(string Name) : IRequest<Result<UniversityDto>>;

