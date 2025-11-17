//using JadaraITKnowledgeSystem.Application.DTOs;
//using JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Dtos;
//using JadaraITKnowledgeSystem.Application.Interfaces.Services.Generic;

//namespace JadaraITKnowledgeSystem.Application.Interfaces.Services
//{
//    public interface IQuizService : IGenericService<QuizDto>
//    {
//        Task AddQuestionAsync(int quizId, QuestionWithChoicesDto questionDto);
//        Task SubmitAttemptAsync(int quizId, int userId, decimal score);
//        Task LikeQuizAsync(int quizId, int userId);
//        Task DislikeQuizAsync(int quizId, int userId);
//        //Task<OperationResult<QuizDto>> CreateQuizAsync(QuizCreateDto quizCreateDto);
//        Task<bool> CreateQuizAsync(QuizCreateDto quizCreateDto);
//    }
//}
