//using AutoMapper;
//using JadaraITKnowledgeSystem.Application.DTOs;
//using JadaraITKnowledgeSystem.Application.Interfaces.Repositories.Generic;
//using JadaraITKnowledgeSystem.Application.Interfaces.Repositories.UnitOfWrok;
//
//using JadaraITKnowledgeSystem.Application.Services.Generic;
//using JadaraITKnowledgeSystem.Domain.Courses.Entites;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace JadaraITKnowledgeSystem.Application.Services
//{
//    public class CourseMaterialService : GenericService<CourseMaterial, CourseMaterialDto>, ICourseMaterialService
//    {
//        private readonly IGenericRepository<CourseMaterial> _courseMaterialRepository;
//        private readonly IUnitOfWork _unitOfWork;
//        public CourseMaterialService(IGenericRepository<CourseMaterial> courseMaterialRepository, IUnitOfWork unitOfWork, IMapper mapper)
//            : base(courseMaterialRepository, unitOfWork, mapper)
//        {
//            _courseMaterialRepository = courseMaterialRepository;
//            _unitOfWork = unitOfWork;
//        }
//    }
//}
