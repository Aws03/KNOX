//using AutoMapper;
//using JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Dtos;
//using JadaraITKnowledgeSystem.Application.Interfaces.Repositories.Generic;
//using JadaraITKnowledgeSystem.Application.Interfaces.Repositories.UnitOfWrok;
//
//using JadaraITKnowledgeSystem.Application.Services.Generic;
//using JadaraITKnowledgeSystem.Domain.Quizzes.Entites;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace JadaraITKnowledgeSystem.Application.Services
//{
//    public class QuestionService : GenericService<Question, QuestionDto>, IQuestionService
//    {
//        private readonly IGenericRepository<Question> _questionRepository;
//        private readonly IUnitOfWork _unitOfWork;
//        public QuestionService(IGenericRepository<Question> questionRepository, IUnitOfWork unitOfWork, IMapper mapper)
//            : base(questionRepository, unitOfWork, mapper)
//        {
//            _questionRepository = questionRepository;
//            _unitOfWork = unitOfWork;
//        }
//    }
//}
