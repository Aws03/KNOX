using JadaraITKnowledgeSystem.Application.Interfaces.Repositories;
using JadaraITKnowledgeSystem.Domain.Universities.Entities;
using JadaraITKnowledgeSystem.Infrastructure.Persistence.Context;
using JadaraITKnowledgeSystem.Infrastructure.Repositories.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Infrastructure.Repositories
{
    public class FacultyRepository : GenericRepository<Faculty>, IFacultyRepository
    {
        public FacultyRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
