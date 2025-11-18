using JadaraITKnowledgeSystem.Application.Interfaces.Repositories;
using JadaraITKnowledgeSystem.Domain.Courses.Entites;
using JadaraITKnowledgeSystem.Infrastructure.Persistence.Context;
using JadaraITKnowledgeSystem.Infrastructure.Repositories.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Infrastructure.Repositories
{
    public class CourseRequirementMappingRepository : GenericRepository<CourseRequirementMapping>, ICourseRequirementMappingRepository
    {
        public CourseRequirementMappingRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
