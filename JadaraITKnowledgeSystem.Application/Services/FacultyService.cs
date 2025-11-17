//using AutoMapper;
//using JadaraITKnowledgeSystem.Application.DTOs;
//using JadaraITKnowledgeSystem.Application.Interfaces.Repositories.Generic;
//using JadaraITKnowledgeSystem.Application.Interfaces.Repositories.UnitOfWrok;
//
//using JadaraITKnowledgeSystem.Application.Services.Generic;
//using JadaraITKnowledgeSystem.Domain.Universities.Entities;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace JadaraITKnowledgeSystem.Application.Services
//{
//    public class FacultyService : GenericService<Faculty, FacultyDto>, IFacultyService
//    {
//        private readonly IGenericRepository<Faculty> _facultyRepository;
//        private readonly IUnitOfWork _unitOfWork;
//        public FacultyService(IGenericRepository<Faculty> facultyRepository, IUnitOfWork unitOfWork, IMapper mapper) : base(facultyRepository, unitOfWork, mapper)
//        {
//            _facultyRepository = facultyRepository;
//            _unitOfWork = unitOfWork;
//        }
//    }
//}
