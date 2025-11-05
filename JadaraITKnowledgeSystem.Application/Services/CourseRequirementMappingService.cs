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
    public class CourseRequirementMappingService : GenericService<CourseRequirementMapping, CourseRequirementMappingDto>, ICourseRequirementMappingService
    {
        private readonly IGenericRepository<CourseRequirementMapping> _courseRequirementMappingRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CourseRequirementMappingService(IGenericRepository<CourseRequirementMapping> courseRequirementMappingRepository, IUnitOfWork unitOfWork, IMapper mapper)
            : base(courseRequirementMappingRepository, unitOfWork, mapper)
        {
            _courseRequirementMappingRepository = courseRequirementMappingRepository;
            _unitOfWork = unitOfWork;

        }
    }
}
