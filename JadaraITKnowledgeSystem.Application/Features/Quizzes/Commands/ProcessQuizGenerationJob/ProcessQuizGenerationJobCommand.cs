using System.Collections.Generic;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Quizzes.Commands.ProcessQuizGenerationJob;

public sealed record ProcessQuizGenerationJobCommand(int JobId) 
    : IRequest<Result<List<QuizDto>>>;
