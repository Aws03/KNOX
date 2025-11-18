using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Common.Queries;
using JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Queries.GetQuizzes
{
    public sealed record GetQuizzesByCourseIdQuery(
    int CourseId,
    int PageNumber = 1,
    int PageSize = 10
    ) : PaginatedQuery<Result<PaginatedList<QuizSummaryDto>>>(PageNumber, PageSize);
}
