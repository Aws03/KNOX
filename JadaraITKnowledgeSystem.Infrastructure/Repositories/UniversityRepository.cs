using JadaraITKnowledgeSystem.Application.Interfaces.Repositories;
using JadaraITKnowledgeSystem.Domain.Universities;
using JadaraITKnowledgeSystem.Infrastructure.Persistence.Context;
using JadaraITKnowledgeSystem.Infrastructure.Repositories.Generic;

namespace JadaraITKnowledgeSystem.Infrastructure.Repositories
{
    public class UniversityRepository : GenericRepository<University>, IUniversityRepository
    {
        public UniversityRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
