using JadaraITKnowledgeSystem.Application.Common.Behaviours;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Quizzes.Commands.ProcessQuizGenerationJob;

// Runs for as long as the AI calls take; each quiz it creates commits on its own and
// job status changes are saved immediately so pollers can see progress.
public sealed record ProcessQuizGenerationJobCommand(int JobId)
    : IRequest<Result<List<QuizDto>>>, INonTransactionalCommand;
