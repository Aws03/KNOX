using JadaraITKnowledgeSystem.Application.Fetures.Courses.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Queries.GetCourseContentByFolder
{
    public sealed record GetCourseContentsQuery(int CourseId, int? FolderId)
    : IRequest<Result<CourseContentsDto>>;

}
