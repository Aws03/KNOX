using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Common.Queries;
using JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;

namespace JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Queries.GetQuizzes;

public sealed record GetQuizzesByCourseIdQuery(
int CourseId,
int? UserId = null,
int PageNumber = 1,
int PageSize = 10
) : PaginatedQuery<Result<PaginatedList<QuizSummaryDto>>>(PageNumber, PageSize);
