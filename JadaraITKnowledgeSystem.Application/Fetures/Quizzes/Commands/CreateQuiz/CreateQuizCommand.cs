using JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using System.Collections.Generic;

namespace JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Commands.CreateQuiz;

public sealed record CreateQuizCommand(string Title,
    int WriterId,
    int CourseId,
    string? Description,
    List<CreateQuestionDto> Questions,
    IEnumerable<string>? Tags
) : IRequest<Result<QuizDto>>;
