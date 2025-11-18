//using AutoMapper;
//using JadaraITKnowledgeSystem.Application.DTOs;
//using JadaraITKnowledgeSystem.Application.Interfaces.Repositories.Generic;
//using JadaraITKnowledgeSystem.Application.Interfaces.Repositories.UnitOfWrok;
//
//using JadaraITKnowledgeSystem.Application.Services.Generic;
//using JadaraITKnowledgeSystem.Domain.Courses;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace JadaraITKnowledgeSystem.Application.Services
//{
//    public class CourseService : GenericService<Course, CourseDto>, ICourseService
//    {
//        private readonly IGenericRepository<Course> _courseRepository;
//        private readonly IUnitOfWork _unitOfWork;
//        public CourseService(IGenericRepository<Course> repository, IUnitOfWork unitOfWork, IMapper mapper) : base(repository, unitOfWork, mapper)
//        {
//            _courseRepository = repository;
//            _unitOfWork = unitOfWork;
//        }
//    }
//}
