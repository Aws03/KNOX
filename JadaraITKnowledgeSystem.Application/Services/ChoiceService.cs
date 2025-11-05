using AutoMapper;
using JadaraITKnowledgeSystem.Application.DTOs;
using JadaraITKnowledgeSystem.Application.Interfaces.Repositories.Generic;
using JadaraITKnowledgeSystem.Application.Interfaces.Repositories.UnitOfWrok;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Application.Services.Generic;
using JadaraITKnowledgeSystem.Domain.Entities;


namespace JadaraITKnowledgeSystem.Application.Services
{
    public class ChoiceService : GenericService<Choice, ChoiceDto>, IChoiceService
    {
        private readonly IGenericRepository<Choice> _choiceRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ChoiceService(IGenericRepository<Choice> choiceRepository, IUnitOfWork unitOfWork, IMapper mapper)
            : base(choiceRepository, unitOfWork, mapper)
        {
            _choiceRepository = choiceRepository;
            _unitOfWork = unitOfWork;
        }
    }
}
