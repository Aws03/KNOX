using JadaraITKnowledgeSystem.Application.Features.Universities.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Universities.Commands.UpdateUniversity;

public sealed record UpdateUniversityCommand(int Id, string Name) : IRequest<Result<UniversityDto>>;
