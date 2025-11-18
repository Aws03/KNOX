using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Application.Interfaces.Repositories;
using JadaraITKnowledgeSystem.Application.Interfaces.Repositories.UnitOfWrok;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Quizzes.Entites;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Commands.AddReaction
{
    public sealed class AddReactionCommandHandler
        (IApplicationDbContext context,ILogger<AddReactionCommandHandler> logger)
        : IRequestHandler<AddReactionCommand, Result<Success>>
    {
        private readonly IApplicationDbContext _context = context;
        private readonly ILogger<AddReactionCommandHandler> _logger = logger;

        public async Task<Result<Success>> Handle(AddReactionCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Adding reaction -> QuizId={QuizId}, UserId={UserId}, ReactionType={ReactionType}",
                request.QuizId, request.UserId, request.ReactionType);

            // Load quiz with reactions
            var quiz = await _context.Quizzes
                .Include(q => q.Reactions)
                .FirstOrDefaultAsync(q => q.Id == request.QuizId, cancellationToken);

            if (quiz is null)
            {
                _logger.LogWarning("Quiz not found: {QuizId}", request.QuizId);
                return Error.NotFound($"Quiz with id {request.QuizId} is not found");
            }

            var reaction = UserReaction.Create(request.UserId,request.QuizId,request.ReactionType);

            var result = quiz.AddReaction(reaction.Value);

            if (result.IsError)
            {
                _logger.LogWarning(
                    "Failed to add reaction to QuizId={QuizId}. Error={Error}",
                    request.QuizId, result.Errors);

                return result.Errors;
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Reaction added successfully for QuizId={QuizId}", request.QuizId);

            return Result.Success;
        }
    }
}
