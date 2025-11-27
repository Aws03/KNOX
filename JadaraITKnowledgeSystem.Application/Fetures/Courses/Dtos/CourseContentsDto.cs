using JadaraITKnowledgeSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Dtos
{
    public sealed record CourseContentsDto(
    int CourseId,
    int? FolderId,
    List<FolderDto> Folders,
    List<CourseMaterialDto> Materials
);

}
