using JadaraITKnowledgeSystem.Application.Interfaces.Repositories.Generic;
using JadaraITKnowledgeSystem.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Application.Interfaces.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
    }
}
