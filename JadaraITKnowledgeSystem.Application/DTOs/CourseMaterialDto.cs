using JadaraITKnowledgeSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Application.DTOs
{
    public class CourseMaterialDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ContentUrl { get; set; }
        public int CourseId { get; set; }
        public string? Description { get; set; }
    }
}
