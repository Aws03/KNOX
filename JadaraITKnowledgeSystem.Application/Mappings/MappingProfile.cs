using AutoMapper;
using JadaraITKnowledgeSystem.Application.DTOs;
using JadaraITKnowledgeSystem.Domain.Entities;

namespace JadaraITKnowledgeSystem.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Question, QuestionDto>().ReverseMap();
            CreateMap<Choice, ChoiceDto>().ReverseMap();
            CreateMap<Quiz, QuizDto>().ReverseMap();
            CreateMap<UserReaction, UserReactionDto>().ReverseMap();
            CreateMap<QuizAttempt, QuizAttemptDto>().ReverseMap();
        }
    }
}
