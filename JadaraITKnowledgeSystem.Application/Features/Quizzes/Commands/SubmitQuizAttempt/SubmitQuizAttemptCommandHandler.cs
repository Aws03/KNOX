using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Quizzes.Entites;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Quizzes.Commands.SubmitQuizAttempt;

public sealed class SubmitQuizAttemptCommandHandler
    (IApplicationDbContext context, ICurrentUserService currentUser, ILogger<SubmitQuizAttemptCommandHandler> logger)
    : IRequestHandler<SubmitQuizAttemptCommand, Result<QuizAttemptDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly ICurrentUserService _currentUser = currentUser;
    private readonly ILogger<SubmitQuizAttemptCommandHandler> _logger = logger;

    public async Task<Result<QuizAttemptDto>> Handle(SubmitQuizAttemptCommand request, CancellationToken cancellationToken)
    {
        var email = _currentUser.Email;
        if (string.IsNullOrWhiteSpace(email))
        {
            _logger.LogWarning("[SubmitQuizAttempt] Unauthorized attempt - no email on current user");
            return Error.Unauthorized("User.NotAuthenticated", "User is not authenticated.");
        }

        var domainUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.Address == email, cancellationToken);

        if (domainUser is null)
        {
            _logger.LogWarning("[SubmitQuizAttempt] Domain user not found for email {Email}", email);
            return Error.NotFound("User.NotFound", "User not found");
        }

        var quiz = await _context.Quizzes
            .Include(q => q.Attempts)
            .FirstOrDefaultAsync(q => q.Id == request.QuizId, cancellationToken);

        if (quiz is null)
        {
            _logger.LogWarning("[SubmitQuizAttempt] Quiz not found: {QuizId}", request.QuizId);
            return Error.NotFound("Quiz.NotFound", $"Quiz with id {request.QuizId} is not found");
        }

        var attemptResult = QuizAttempt.Create(request.QuizId, domainUser.Id, request.Score);
        if (attemptResult.IsError)
        {
            return attemptResult.Errors;
        }

        quiz.AddOrUpdateAttempt(attemptResult.Value);

        await _context.SaveChangesAsync(cancellationToken);

        var savedAttempt = quiz.Attempts.First(a => a.UserId == domainUser.Id);

        _logger.LogInformation(
            "Quiz attempt recorded. QuizId={QuizId}, UserId={UserId}, Score={Score}",
            request.QuizId, domainUser.Id, request.Score);

        return new QuizAttemptDto
        {
            Id = savedAttempt.Id,
            QuizId = savedAttempt.QuizId,
            UserId = savedAttempt.UserId,
            Score = savedAttempt.Score,
            AttemptDate = savedAttempt.AttemptDate
        };
    }
}
