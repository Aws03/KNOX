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
    public class QuestionService : GenericService<Question, QuestionDto>, IQuestionService
    {
        private readonly IGenericRepository<Question> _questionRepository;
        private readonly IUnitOfWork _unitOfWork;
        public QuestionService(IGenericRepository<Question> questionRepository, IUnitOfWork unitOfWork, IMapper mapper)
            : base(questionRepository, unitOfWork, mapper)
        {
            _questionRepository = questionRepository;
            _unitOfWork = unitOfWork;
        }
    }
}
