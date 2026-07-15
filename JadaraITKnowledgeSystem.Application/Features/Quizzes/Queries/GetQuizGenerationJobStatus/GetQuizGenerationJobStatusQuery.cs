using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Quizzes.Queries.GetQuizGenerationJobStatus;

public sealed record GetQuizGenerationJobStatusQuery(int JobId) 
    : IRequest<Result<QuizGenerationJobDto>>;
