using JadaraITKnowledgeSystem.Application.Fetures.Universities.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Fetures.Universities.Commands.CreateUniversity;

public sealed record CreateUniversityCommand(string Name) : IRequest<Result<UniversityDto>>;

