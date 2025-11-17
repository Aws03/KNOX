using JadaraITKnowledgeSystem.Application.Fetures.Faculties.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Application.Fetures.Universities.Dtos
{
    public sealed record UniversityDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = String.Empty;
        public IReadOnlyList<FacultyDto>? Faculties { get; init; }
    }
}
