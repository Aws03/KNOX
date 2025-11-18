using System;
using System.Collections.Generic;
using System.Text;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Commands.CreateCourseMaterial
{
    public sealed record CreateCourseMaterialCommand(
    string Title,
    IFormFile File,
    int CourseId,
    string? Description
) : IRequest<Result<int>>;

}
