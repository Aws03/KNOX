using JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Quizzes;
using JadaraITKnowledgeSystem.Domain.Quizzes.Entites;
using MediatR;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Commands.CreateQuiz
{
    public sealed class CreateQuizCommandHandler
        (IApplicationDbContext context,
         ILogger<CreateQuizCommandHandler> logger)
        : IRequestHandler<CreateQuizCommand, Result<QuizDto>>
    {
        private readonly IApplicationDbContext _context = context;
        private readonly ILogger<CreateQuizCommandHandler> _logger = logger;

        public async Task<Result<QuizDto>> Handle(
            CreateQuizCommand command,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating quiz: {Title}", command.Title);

            var quizResult = Quiz.Create(
                courseId: command.CourseId,
                writerId: command.WriterId,
                title: command.Title,
                description: command.Description
            );

            if (!quizResult.IsSuccess)
                return quizResult.Errors;

            var quiz = quizResult.Value;

            foreach (var q in command.Questions)
            {
                var questionResult = Question.Create(
                    quizId: 0,
                    type: q.Type,
                    text: q.Text
                );

                if (!questionResult.IsSuccess)
                    return questionResult.Errors;

                var question = questionResult.Value;
                quiz.AddQuestion(question);

                foreach (var c in q.Choices)
                {
                    var choiceResult = Choice.Create(0,c.Text, c.IsCorrect);
                        

                    if (!choiceResult.IsSuccess)
                        return choiceResult.Errors;

                    question.AddChoice(choiceResult.Value);
                }
            }

            await _context.Quizzes.AddAsync(quiz, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return quiz.ToDto();
        }
    }
}
