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
//    public class MajorService : GenericService<Major, MajorDto>, IMajorService
//    {
//        private readonly IGenericRepository<Major> _majorRepository;
//        private readonly IUnitOfWork _unitOfWork;
//        public MajorService(IGenericRepository<Major> majorRepository, IUnitOfWork unitOfWork, IMapper mapper) : base(majorRepository, unitOfWork, mapper)
//        {
//            _majorRepository = majorRepository;
//            _unitOfWork = unitOfWork;
//        }

//    }
//}
