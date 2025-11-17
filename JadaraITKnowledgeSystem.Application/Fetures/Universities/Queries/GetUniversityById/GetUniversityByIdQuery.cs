using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Common.Queries;
using JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Application.Fetures.Universities.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JadaraITKnowledgeSystem.Application.Fetures.Universities.Queries.GetUniversityById
{
    public sealed record GetUniversityByIdQuery(int UniversityId)
        : IRequest<Result<UniversityDto>>;

}
