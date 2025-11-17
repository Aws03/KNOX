//using AutoMapper;
//using JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Dtos;
//using JadaraITKnowledgeSystem.Application.Interfaces.Repositories.Generic;
//using JadaraITKnowledgeSystem.Application.Interfaces.Repositories.UnitOfWrok;
//
//using JadaraITKnowledgeSystem.Application.Services.Generic;
//using JadaraITKnowledgeSystem.Domain.Quizzes.Entites;


//namespace JadaraITKnowledgeSystem.Application.Services
//{
//    public class ChoiceService : GenericService<Choice, ChoiceDto>, IChoiceService
//    {
//        private readonly IGenericRepository<Choice> _choiceRepository;
//        private readonly IUnitOfWork _unitOfWork;

//        public ChoiceService(IGenericRepository<Choice> choiceRepository, IUnitOfWork unitOfWork, IMapper mapper)
//            : base(choiceRepository, unitOfWork, mapper)
//        {
//            _choiceRepository = choiceRepository;
//            _unitOfWork = unitOfWork;
//        }
//    }
//}
