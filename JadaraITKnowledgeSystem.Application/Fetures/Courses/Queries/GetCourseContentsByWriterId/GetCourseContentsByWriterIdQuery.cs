using JadaraITKnowledgeSystem.Application.Fetures.Courses.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Queries.GetCourseContentsByWriterId
{
    public sealed record GetCourseContentsByWriterIdQuery(int CourseId, int? FolderId)
        : IRequest<Result<CourseContentsDto>>;
}
