using System.Text;
using JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Queries.GetCourseContents
{
    public sealed record GetCourseContentsQuery(int CourseId, int? FolderId)
    : IRequest<Result<CourseContentsDto>>;

}
