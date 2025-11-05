using AutoMapper;
using JadaraITKnowledgeSystem.Application.DTOs;
using JadaraITKnowledgeSystem.Application.Interfaces.Repositories.Generic;
using JadaraITKnowledgeSystem.Application.Interfaces.Repositories.UnitOfWrok;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Application.Services.Generic;
using JadaraITKnowledgeSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Application.Services
{
    public class UniversityService : GenericService<University, UniversityDto>, IUniversityService
    {
        private readonly IGenericRepository<University> _universityRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UniversityService(IGenericRepository<University> universityRepository, IUnitOfWork unitOfWork, IMapper mapper) 
            : base(universityRepository, unitOfWork, mapper)
        {
            _universityRepository = universityRepository;
            _unitOfWork = unitOfWork;
        }
    }
}
