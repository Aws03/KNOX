using JadaraITKnowledgeSystem.Application.Common.Security;
using JadaraITKnowledgeSystem.Application.Features.Users.Dtos;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Quizzes.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JadaraITKnowledgeSystem.Application.Features.Users.Queries.GetWriterStatistics;

public sealed class GetWriterStatisticsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    : IRequestHandler<GetWriterStatisticsQuery, Result<WriterStatisticsDto>>
{
    public async Task<Result<WriterStatisticsDto>> Handle(GetWriterStatisticsQuery request, CancellationToken cancellationToken)
    {
        var writerId = request.WriterId;

        // Writers see their own dashboard; admins may look at anyone's.
        var isAdmin = Roles.Highest(currentUser.Roles) is Roles.SuperAdmin or Roles.Admin;
        if (!isAdmin && currentUser.DomainUserId != writerId)
            return Error.Forbidden("WriterStatistics.Forbidden", "You can only view your own statistics.");

        var writerEmail = await context.Users
            .Where(u => u.Id == writerId)
            .Select(u => u.Email.Address)
            .FirstOrDefaultAsync(cancellationToken);

        if (writerEmail is null)
            return Error.NotFound("User.NotFound", $"User {writerId} not found.");

        // Materials have no writer FK; the audit interceptor stamps CreatedBy with the author's
        // email as it appears in their token, while Users stores the normalized (upper-case) form.
        var totalMaterials = await context.CourseMaterials.CountAsync(
            m => m.CreatedBy.ToUpper() == writerEmail, cancellationToken);

        var quizIds = context.Quizzes.Where(q => q.WriterId == writerId).Select(q => q.Id);
        var questionIds = context.Questions.Where(q => quizIds.Contains(q.QuizId)).Select(q => q.Id);

        return new WriterStatisticsDto
        {
            TotalMaterials = totalMaterials,
            TotalQuizzes = await quizIds.CountAsync(cancellationToken),
            TotalQuizAttempts = await context.QuizAttempts.CountAsync(a => quizIds.Contains(a.QuizId), cancellationToken),
            TotalQuizLikes = await context.UserReactions.CountAsync(
                r => quizIds.Contains(r.QuizId) && r.ReactionType == ReactionType.Like, cancellationToken),
            TotalQuizDislikes = await context.UserReactions.CountAsync(
                r => quizIds.Contains(r.QuizId) && r.ReactionType == ReactionType.Dislike, cancellationToken),
            TotalQuizQuestions = await questionIds.CountAsync(cancellationToken),
            TotalQuizChoices = await context.Choices.CountAsync(c => questionIds.Contains(c.QuestionId), cancellationToken)
        };
    }
}
