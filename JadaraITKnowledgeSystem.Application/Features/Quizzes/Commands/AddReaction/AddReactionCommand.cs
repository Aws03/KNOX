using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Quizzes.Enums;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Quizzes.Commands.AddReaction;

// UserId is deliberately not a parameter here - it's resolved server-side from
// the authenticated caller (see AddReactionCommandHandler), so a client can
// never spoof a reaction on another user's behalf.
public sealed record AddReactionCommand(
    int QuizId,
    ReactionType ReactionType
) : IRequest<Result<ReactionResultDto>>;
