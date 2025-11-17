//using AutoMapper;
//using JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Dtos;
//using JadaraITKnowledgeSystem.Application.Interfaces.Repositories.Generic;
//using JadaraITKnowledgeSystem.Application.Interfaces.Repositories.UnitOfWrok;
//
//using JadaraITKnowledgeSystem.Application.Services.Generic;
//using JadaraITKnowledgeSystem.Domain.Quizzes.Entites;


//namespace JadaraITKnowledgeSystem.Application.Services
//{
//    public class QuizAttemptService : GenericService<QuizAttempt, QuizAttemptDto>, IQuizAttemptService
//    {
//        private readonly IGenericRepository<QuizAttempt> _quizAttemptRepository;
//        private readonly IUnitOfWork _unitOfWork;
//        public QuizAttemptService(IGenericRepository<QuizAttempt> quizAttemptRepository, IUnitOfWork unitOfWork, IMapper mapper)
//            : base(quizAttemptRepository, unitOfWork, mapper)
//        {
//            _quizAttemptRepository = quizAttemptRepository;
//            _unitOfWork = unitOfWork;
//        }
//    }
//}
