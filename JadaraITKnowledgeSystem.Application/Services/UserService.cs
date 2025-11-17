//using AutoMapper;
//using JadaraITKnowledgeSystem.Application.DTOs;
//using JadaraITKnowledgeSystem.Application.Interfaces.Repositories.Generic;
//using JadaraITKnowledgeSystem.Application.Interfaces.Repositories.UnitOfWrok;
//
//using JadaraITKnowledgeSystem.Application.Services.Generic;
//using JadaraITKnowledgeSystem.Domain.Users;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace JadaraITKnowledgeSystem.Application.Services
//{
//    public class UserService : GenericService<User, UserDto>, IUserService
//    {
//        private readonly IGenericRepository<User> _userRepository;
//        private readonly IUnitOfWork _unitOfWork;
//        public UserService(IGenericRepository<User> userRepository, IUnitOfWork unitOfWork, IMapper mapper)
//            : base(userRepository, unitOfWork, mapper)
//        {
//            _userRepository = userRepository;
//            _unitOfWork = unitOfWork;
//        }
//    }
//}
