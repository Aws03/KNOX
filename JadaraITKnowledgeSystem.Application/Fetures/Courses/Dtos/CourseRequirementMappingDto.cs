using JadaraITKnowledgeSystem.Domain.Courses.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Application.DTOs
{
    public sealed record CourseRequirementMappingDto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int MajorId { get; set; }
        public RequirementType RequirementType { get; set; } // (University, Faculty, Major, Remedial)
        public RequirementNature RequirementNature { get; set; } // (Compulsory, Elective)
    }
}
