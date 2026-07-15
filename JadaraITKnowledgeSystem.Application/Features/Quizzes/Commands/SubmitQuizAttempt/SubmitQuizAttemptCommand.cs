using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Quizzes.Commands.SubmitQuizAttempt;

// UserId is deliberately not a parameter here - it's resolved server-side from
// the authenticated caller, same as AddReactionCommand, so a client can never
// record an attempt on another user's behalf.
public sealed record SubmitQuizAttemptCommand(
    int QuizId,
    decimal Score
) : IRequest<Result<QuizAttemptDto>>;
