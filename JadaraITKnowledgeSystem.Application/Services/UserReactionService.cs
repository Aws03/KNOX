//using AutoMapper;
//using JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Dtos;
//using JadaraITKnowledgeSystem.Application.Interfaces.Repositories.Generic;
//using JadaraITKnowledgeSystem.Application.Interfaces.Repositories.UnitOfWrok;
//
//using JadaraITKnowledgeSystem.Application.Services.Generic;
//using JadaraITKnowledgeSystem.Domain.Quizzes.Entites;


//namespace JadaraITKnowledgeSystem.Application.Services
//{
//    public class UserReactionService : GenericService<UserReaction, UserReactionDto>, IUserReactionService
//    {
//        private readonly IGenericRepository<UserReaction> _userReactionRepository;
//        private readonly IUnitOfWork _unitOfWork;
//        public UserReactionService(IGenericRepository<UserReaction> userReactionRepository, IUnitOfWork unitOfWork, IMapper mapper)
//            : base(userReactionRepository, unitOfWork, mapper)
//        {
//            _userReactionRepository = userReactionRepository;
//            _unitOfWork = unitOfWork;
//        }
//    }
//}
