using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Quizzes.Queries.GetQuizzesByWriterId;

public sealed class GetQuizzesByWriterIdQueryHandler
    : IRequestHandler<GetQuizzesByWriterIdQuery, Result<PaginatedList<QuizSummaryDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<GetQuizzesByWriterIdQueryHandler> _logger;

    public GetQuizzesByWriterIdQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        ILogger<GetQuizzesByWriterIdQueryHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<QuizSummaryDto>>> Handle(
        GetQuizzesByWriterIdQuery request,
        CancellationToken token)
    {
        var userId = _currentUser.UserId;
        if (!userId.HasValue)
            return Error.Unauthorized("User.NotAuthenticated", "User is not authenticated.");

        _logger.LogInformation("Getting quizzes for Course {CourseId} created by Writer {WriterId}", request.CourseId, userId.Value);

        var query = _context.Quizzes
            .AsNoTracking()
            .Where(q => q.CourseId == request.CourseId && q.WriterId == userId.Value)
            .OrderByDescending(q => q.CreatedAt)
            .Select(q => new QuizSummaryDto
            {
                Id = q.Id,
                Title = q.Title,
                Likes = q.Likes,
                WriterName = q.Writer.Name.Value,
                CreatedAt = q.CreatedAt,
                LastAttemptScore = q.Attempts
                    .Where(a => a.UserId == userId.Value)
                    .OrderByDescending(a => a.AttemptDate)
                    .Select(a => (decimal?)a.Score)
                    .FirstOrDefault()
            });

        var paginated = await PaginatedList<QuizSummaryDto>.CreateAsync(
            query,
            request.PageNumber,
            request.PageSize,
            token
        );

        return paginated;
    }
}
