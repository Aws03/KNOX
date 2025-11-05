using JadaraITKnowledgeSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Application.DTOs
{
    public class MajorDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int FacultyId { get; set; }
    }
}
