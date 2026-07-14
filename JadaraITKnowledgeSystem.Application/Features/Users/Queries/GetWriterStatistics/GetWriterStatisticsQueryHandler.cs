using JadaraITKnowledgeSystem.Application.Features.Users.Dtos;
using JadaraITKnowledgeSystem.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Application.Features.Users.Queries.GetWriterStatistics
{
    public class GetWriterStatisticsQueryHandler : IRequestHandler<GetWriterStatisticsQuery, WriterStatisticsDto>
    {
        private readonly IApplicationDbContext _context;
        public GetWriterStatisticsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<WriterStatisticsDto> Handle(GetWriterStatisticsQuery request, CancellationToken cancellationToken)
        {
            var writerId = request.WriterId;

            var totalMaterials = await _context.CourseMaterials.CountAsync(m => m.Id == writerId, cancellationToken);
            var quizzes = _context.Quizzes.Where(q => q.WriterId == writerId);
            var totalQuizzes = await quizzes.CountAsync(cancellationToken);
            var totalQuizAttempts = await _context.QuizAttempts.CountAsync(a => quizzes.Select(q => q.Id).Contains(a.QuizId), cancellationToken);
            var totalQuizLikes = await _context.UserReactions.CountAsync(r => quizzes.Select(q => q.Id).Contains(r.QuizId) && r.ReactionType == Domain.Quizzes.Enums.ReactionType.Like, cancellationToken);
            var totalQuizDislikes = await _context.UserReactions.CountAsync(r => quizzes.Select(q => q.Id).Contains(r.QuizId) && r.ReactionType == Domain.Quizzes.Enums.ReactionType.Dislike, cancellationToken);
            var totalQuizQuestions = await _context.Questions.CountAsync(q => quizzes.Select(x => x.Id).Contains(q.QuizId), cancellationToken);
            var totalQuizChoices = await _context.Choices.CountAsync(c => quizzes.Select(q => q.Id).Contains(c.Id), cancellationToken);

            return new WriterStatisticsDto
            {
                TotalMaterials = totalMaterials,
                TotalQuizzes = totalQuizzes,
                TotalQuizAttempts = totalQuizAttempts,
                TotalQuizLikes = totalQuizLikes,
                TotalQuizDislikes = totalQuizDislikes,
                TotalQuizQuestions = totalQuizQuestions,
                TotalQuizChoices = totalQuizChoices
            };
        }
    }
}
