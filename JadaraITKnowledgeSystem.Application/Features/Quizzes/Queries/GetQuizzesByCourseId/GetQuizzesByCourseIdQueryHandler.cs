using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Quizzes.Queries.GetQuizzesByCourseId;

public sealed class GetQuizzesByCourseIdQueryHandler
    : IRequestHandler<GetQuizzesByCourseIdQuery, Result<PaginatedList<QuizSummaryDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<GetQuizzesByCourseIdQueryHandler> _logger;

    public GetQuizzesByCourseIdQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        ILogger<GetQuizzesByCourseIdQueryHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<QuizSummaryDto>>> Handle(
        GetQuizzesByCourseIdQuery request,
        CancellationToken token)
    {
        _logger.LogInformation("Getting quizzes for Course {CourseId}", request.CourseId);

        // The caller's own last score; anonymous callers get none.
        var userId = _currentUser.DomainUserId;

        var query = _context.Quizzes
            .AsNoTracking()
            .Where(q => q.CourseId == request.CourseId)
            .OrderByDescending(q => q.CreatedAt)
            .Select(q => new QuizSummaryDto
            {
                Id = q.Id,
                Title = q.Title,
                Likes = q.Likes,
                WriterName = q.Writer.Name.Value,
                CreatedAt = q.CreatedAt,
                LastAttemptScore = userId == null
                    ? (decimal?)null
                    : q.Attempts
                        .Where(a => a.UserId == userId)
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
