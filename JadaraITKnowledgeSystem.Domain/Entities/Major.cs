using JadaraITKnowledgeSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Domain.Entities
{
    public class Major : AuditableEntity
    {
        //[Key]
        //public int MajorId { get; private set; }
        [Required]
        [MaxLength(120)]
        public string Name { get; private set; }

        [ForeignKey(nameof(Faculty))]
        public int FacultyId { get; private set; }
        public Faculty Faculty { get; private set; }

        private readonly List<CourseRequirementMapping> _courseRequirements = new();
        public IReadOnlyCollection<CourseRequirementMapping> CourseRequirements => _courseRequirements.AsReadOnly();

        private Major() { }

        public Major(string name,int facultyId)
        {
            SetName(name);
            SetFacultyId(facultyId);
        }

        private void SetFacultyId(int facultyId)
        {
            if(facultyId <= 0)
                throw new ArgumentException(nameof(FacultyId),"FacultyId must be a positive integer");
            FacultyId = facultyId;
        }

        private void SetName(string name)
        {
            if(string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(Name),"Major name could not be null or empty");
            
            if(name.Length > 120)
                throw new ArgumentException(nameof(Name),"Major name must be less than 120 characters");

            Name = name.Trim();
        }
    }
}
