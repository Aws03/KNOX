using JadaraITKnowledgeSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Application.Fetures.Majors.Dtos
{
    public sealed record MajorDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = String.Empty;
        public int FacultyId { get; init; }
        public List<CourseRequirementMappingDto> CourseRequirementMappings { get; init; } = new();
    }
}
