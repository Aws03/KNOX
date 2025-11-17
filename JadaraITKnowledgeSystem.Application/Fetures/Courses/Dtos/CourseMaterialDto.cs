using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Application.DTOs
{
    public sealed record CourseMaterialDto
    {
        public int Id { get; init; }
        public string Title { get; init; } = String.Empty;
        public string ContentUrl { get; init; } = String.Empty;
        public int CourseId { get; init; } 
        public string? Description { get; init; }
    }
}
