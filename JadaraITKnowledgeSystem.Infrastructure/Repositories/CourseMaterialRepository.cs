using JadaraITKnowledgeSystem.Application.Interfaces.Repositories;
using JadaraITKnowledgeSystem.Domain.Entities;
using JadaraITKnowledgeSystem.Infrastructure.Persistence.Context;
using JadaraITKnowledgeSystem.Infrastructure.Repositories.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Infrastructure.Repositories
{
    public class CourseMaterialRepository : GenericRepository<CourseMaterial>, ICourseMaterialRepository
    {
        public CourseMaterialRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
