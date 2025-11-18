//using AutoMapper;
//using JadaraITKnowledgeSystem.Application.DTOs;
//using JadaraITKnowledgeSystem.Application.Interfaces.Repositories;
//using JadaraITKnowledgeSystem.Application.Interfaces.Repositories.UnitOfWrok;
//
//using JadaraITKnowledgeSystem.Application.Services.Generic;
//using JadaraITKnowledgeSystem.Domain.Quizzes.Entites;
//using JadaraITKnowledgeSystem.Domain.Quizzes;
//using JadaraITKnowledgeSystem.Domain.Quizzes.Enums;
//using JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Dtos;


//namespace JadaraITKnowledgeSystem.Application.Services
//{
//    public class QuizService : GenericService<Quiz, QuizDto>, IQuizService
//    {
//        private readonly IQuizRepository _quizRepository;
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;

//        public QuizService(IQuizRepository quizRepository, IUnitOfWork unitOfWork, IMapper mapper)
//            : base(quizRepository, unitOfWork, mapper) // generic CRUD still works
//        {
//            _quizRepository = quizRepository;
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }

//        public async Task AddQuestionAsync(int quizId, QuestionWithChoicesDto questionDto)
//        {
//            //var quiz = await _quizRepository.GetByIdAsync(quizId);
//            //if (quiz == null) throw new KeyNotFoundException("Quiz not found");

//            //var question = new Question(quizId, (QuestionType)questionDto.Type, questionDto.Text);
//            //foreach (var choiceDto in questionDto.Choices)
//            //{
//            //    var choice = new Choice(questionDto.Id, choiceDto.Text, choiceDto.IsCorrect);
//            //    question.AddChoice(choice);
//            //}

//            //quiz.AddQuestion(question);

//            //await _unitOfWork.SaveChangesAsync();
//        }

//        public async Task SubmitAttemptAsync(int quizId, int userId, decimal score)
//        {
//            var quiz = await _quizRepository.GetByIdAsync(quizId);
//            if (quiz == null) throw new KeyNotFoundException("Quiz not found");

//            // Create a QuizAttempt entity
//            var attempt = new QuizAttempt(quizId, userId, score);


//            quiz.AddOrUpdateAttempt(attempt);

//            await _unitOfWork.SaveChangesAsync();
//        }

//        public async Task LikeQuizAsync(int quizId, int userId)
//        {
//            var quiz = await _quizRepository.GetByIdAsync(quizId);
//            if (quiz == null) throw new KeyNotFoundException("Quiz not found");

//            var reaction = new UserReaction(userId, quizId, true);
//            quiz.AddReaction(reaction);

//            await _unitOfWork.SaveChangesAsync();
//        }

//        public async Task DislikeQuizAsync(int quizId, int userId)
//        {
//            var quiz = await _quizRepository.GetByIdAsync(quizId);
//            if (quiz == null) throw new KeyNotFoundException("Quiz not found");

//            var reaction = new UserReaction(userId, quizId, false);
//            quiz.AddReaction(reaction);

//            await _unitOfWork.SaveChangesAsync();
//        }

//        public async Task<bool> CreateQuizAsync(QuizCreateDto quizCreateDto)
//        {
//            //var quiz = new Quiz(quizCreateDto.MaterialId, 1, quizCreateDto.Title);

//            //foreach (var questionDto in quizCreateDto.Questions)
//            //{
//            //    var question = new Question(quiz.Id, (QuestionType)questionDto.Type, questionDto.Text);
//            //    foreach (var choiceDto in questionDto.Choices)
//            //    {
//            //        var choice = new Choice(questionDto.Id, choiceDto.Text, choiceDto.IsCorrect);
//            //        question.AddChoice(choice);
//            //    }
//            //    quiz.AddQuestion(question);
//            //}

//            //await _quizRepository.AddAsync(quiz);
//            //await _unitOfWork.SaveChangesAsync();
//            return true;
//        }
//    }
//}
