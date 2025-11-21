using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Quizzes.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Commands.AddReaction;

public sealed record AddReactionCommand(
    int QuizId,
    int UserId,
    ReactionType ReactionType
) : IRequest<Result<Success>>;
