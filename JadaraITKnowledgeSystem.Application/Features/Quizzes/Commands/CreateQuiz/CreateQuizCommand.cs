using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using System.Collections.Generic;

namespace JadaraITKnowledgeSystem.Application.Features.Quizzes.Commands.CreateQuiz;

public sealed record CreateQuizCommand(string Title,
    int WriterId,
    int CourseId,
    string? Description,
    List<CreateQuestionDto> Questions,
    IEnumerable<string>? Tags
) : IRequest<Result<QuizDto>>;
