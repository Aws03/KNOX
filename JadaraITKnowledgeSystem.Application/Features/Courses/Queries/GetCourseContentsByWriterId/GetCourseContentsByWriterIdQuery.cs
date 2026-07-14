using JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Queries.GetCourseContentsByWriterId
{
    public sealed record GetCourseContentsByWriterIdQuery(int CourseId, int? FolderId)
        : IRequest<Result<CourseContentsDto>>;
}
