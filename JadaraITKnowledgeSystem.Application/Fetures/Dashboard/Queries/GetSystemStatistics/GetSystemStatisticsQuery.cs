using JadaraITKnowledgeSystem.Application.Fetures.Dashboard.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Fetures.Dashboard.Queries.GetSystemStatistics;

public sealed record GetSystemStatisticsQuery(int Months = 6) : IRequest<Result<SystemStatisticsDto>>;
