using JadaraITKnowledgeSystem.Domain.Common;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Courses.Enums;
using JadaraITKnowledgeSystem.Domain.Universities.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Domain.Courses.Entites
{
    public sealed class CourseRequirementMapping : AuditableEntity
    {
        //[Key]
        //public int MappingId { get; private set; }

        [ForeignKey(nameof(Course))]
        public int CourseId { get; private set; }
        public Course Course { get; private set; }

        [ForeignKey(nameof(Major))]
        public int MajorId { get; private set; }
        public Major Major { get; private set; }

        public RequirementType RequirementType { get; private set; } // (University, Faculty, Major, Remedial)
        public RequirementNature RequirementNature { get; private set; } // (Compulsory, Elective)

        private CourseRequirementMapping() { }

        private CourseRequirementMapping(int courseId,
            int majorId,
            RequirementType requirementType,
            RequirementNature requirementNature)
        {

            SetCourse(courseId);
            SetMajor(majorId);
            RequirementType = requirementType;
            RequirementNature = requirementNature;
        }

        public static Result<CourseRequirementMapping> Create(int courseId,
            int majorId,
            RequirementType requirementType,
            RequirementNature requirementNature)
        {
            return new CourseRequirementMapping(courseId,
                majorId,
                requirementType,
                requirementNature);
        }

        private void SetCourse(int courseId)
        {
            if (courseId <= 0) throw new ArgumentOutOfRangeException(nameof(courseId));
            CourseId = courseId;
        }

        private void SetMajor(int majorId)
        {
            if (majorId <= 0) throw new ArgumentOutOfRangeException(nameof(majorId));
            MajorId = majorId;
        }

        public void UpdateRequirementType(RequirementType requirementType)
        {
            RequirementType = requirementType;
        }

        public void UpdateRequirementNature(RequirementNature requirementNature)
        {
            RequirementNature = requirementNature;
        }


    }
}
