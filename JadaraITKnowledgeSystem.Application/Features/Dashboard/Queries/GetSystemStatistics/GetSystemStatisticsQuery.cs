using JadaraITKnowledgeSystem.Application.Features.Dashboard.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Dashboard.Queries.GetSystemStatistics;

public sealed record GetSystemStatisticsQuery(int Months = 6) : IRequest<Result<SystemStatisticsDto>>;
