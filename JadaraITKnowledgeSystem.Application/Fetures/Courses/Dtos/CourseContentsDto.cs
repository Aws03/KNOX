using System.Collections.Generic;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Dtos;

public sealed record CourseContentsDto(
    int CourseId,
    int? FolderId,
    List<FolderDto> Folders,
    List<CourseMaterialDto> Materials
);
