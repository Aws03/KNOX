using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Quizzes.Entites;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Quizzes.Commands.AddReaction;

public sealed class AddReactionCommandHandler
    (IApplicationDbContext context, ICurrentUserService currentUser, ILogger<AddReactionCommandHandler> logger)
    : IRequestHandler<AddReactionCommand, Result<ReactionResultDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly ICurrentUserService _currentUser = currentUser;
    private readonly ILogger<AddReactionCommandHandler> _logger = logger;

    public async Task<Result<ReactionResultDto>> Handle(AddReactionCommand request, CancellationToken cancellationToken)
    {
        var email = _currentUser.Email;
        if (string.IsNullOrWhiteSpace(email))
        {
            _logger.LogWarning("[AddReaction] Unauthorized attempt - no email on current user");
            return Error.Unauthorized("User.NotAuthenticated", "User is not authenticated.");
        }

        var domainUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.Address == email, cancellationToken);

        if (domainUser is null)
        {
            _logger.LogWarning("[AddReaction] Domain user not found for email {Email}", email);
            return Error.NotFound("User.NotFound", "User not found");
        }

        _logger.LogInformation(
            "Adding reaction -> QuizId={QuizId}, UserId={UserId}, ReactionType={ReactionType}",
            request.QuizId, domainUser.Id, request.ReactionType);

        // Load quiz with reactions
        var quiz = await _context.Quizzes
            .Include(q => q.Reactions)
            .FirstOrDefaultAsync(q => q.Id == request.QuizId, cancellationToken);

        if (quiz is null)
        {
            _logger.LogWarning("Quiz not found: {QuizId}", request.QuizId);
            return Error.NotFound($"Quiz with id {request.QuizId} is not found");
        }

        var reaction = UserReaction.Create(domainUser.Id, request.QuizId, request.ReactionType);

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

        return new ReactionResultDto(quiz.Likes, quiz.Dislikes);
    }
}
