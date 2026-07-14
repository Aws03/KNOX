using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Mappers;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Queries.GetQuizzes;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Quizzes.Queries.GetQuizzesByCourse;

public sealed class GetQuizzesByCourseIdQueryHandler
    : IRequestHandler<GetQuizzesByCourseIdQuery, Result<PaginatedList<QuizSummaryDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetQuizzesByCourseIdQueryHandler> _logger;

    public GetQuizzesByCourseIdQueryHandler(IApplicationDbContext context, ILogger<GetQuizzesByCourseIdQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<QuizSummaryDto>>> Handle(
        GetQuizzesByCourseIdQuery request,
        CancellationToken token)
    {
        _logger.LogInformation("Getting quizzes for Course {CourseId}", request.CourseId);

        var userId = request.UserId; // capture for EF translation

        var query = _context.Quizzes
            .AsNoTracking()
            .Where(q => q.CourseId == request.CourseId)
            .OrderByDescending(q => q.CreatedAt)
            .Select(q => new QuizSummaryDto //TODO : Use local mapper later (edit to toSummaryDto())
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
