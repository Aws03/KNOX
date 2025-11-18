using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Common.Queries;
using JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Application.Fetures.Universities.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace JadaraITKnowledgeSystem.Application.Fetures.Universities.Queries.GetUniversities
{
    public sealed record GetUniversitiesQuery(
    int PageNumber = 1,
    int PageSize = 10
    ) : PaginatedQuery<Result<PaginatedList<UniversityDto>>>(PageNumber, PageSize);

}
