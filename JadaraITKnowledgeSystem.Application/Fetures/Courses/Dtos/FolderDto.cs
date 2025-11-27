using JadaraITKnowledgeSystem.Domain.Courses.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Dtos
{
    public class FolderDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public int? ParentFolderId { get; set; }
        public string? Description { get; set; }
    }
}
