using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Mappers;
using JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Queries.GetQuizzes;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Queries.GetQuizzesByCourse
{
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

            var query = _context.Quizzes
                .AsNoTracking()
                .Where(q => q.CourseId == request.CourseId)
                .Include(q => q.Writer) // to get WriterName
                .OrderByDescending(q => q.CreatedAt)
                .Select(q => q.ToSummaryDto());

            var paginated = await PaginatedList<QuizSummaryDto>.CreateAsync(
                query,
                request.PageNumber,
                request.PageSize,
                token
            );

            return paginated;
        }
    }
}
