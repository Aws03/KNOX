using JadaraITKnowledgeSystem.Application.Features.Users.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Users.Queries.GetWriterStatistics;

/// <param name="WriterId">The writer's domain user id.</param>
public sealed record GetWriterStatisticsQuery(int WriterId) : IRequest<Result<WriterStatisticsDto>>;
