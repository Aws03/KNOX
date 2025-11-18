using JadaraITKnowledgeSystem.Application.Fetures.Majors.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Application.Fetures.Faculties.Dtos
{
    public sealed record FacultyDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = String.Empty;
        public int UniversityId { get; init; }
        public List<MajorDto> Majors { get; init; } = new();
    }
}
