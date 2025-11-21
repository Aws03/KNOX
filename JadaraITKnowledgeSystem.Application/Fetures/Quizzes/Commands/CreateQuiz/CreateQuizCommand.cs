using JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Commands.CreateQuiz;

public sealed record CreateQuizCommand(string Title,
    int WriterId,
    int CourseId,
    string? Description,
    List<CreateQuestionDto> Questions
) : IRequest<Result<QuizDto>>;
