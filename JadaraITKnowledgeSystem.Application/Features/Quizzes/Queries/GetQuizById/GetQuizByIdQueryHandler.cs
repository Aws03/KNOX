using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Quizzes.Queries.GetQuizById;

public sealed class GetQuizByIdQueryHandler
(IApplicationDbContext context, ILogger<GetQuizByIdQueryHandler> logger)
: IRequestHandler<GetQuizByIdQuery, Result<QuizDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly ILogger<GetQuizByIdQueryHandler> _logger = logger;

    public async Task<Result<QuizDto>> Handle(
        GetQuizByIdQuery request,
        CancellationToken token)
    {
        _logger.LogInformation("Loading Quiz {QuizId}", request.QuizId);

        var quiz = await _context.Quizzes
            .AsNoTracking()
            .Include(q => q.Writer)
            .Include(q => q.Questions).ThenInclude(q => q.Choices)
            .FirstOrDefaultAsync(q => q.Id == request.QuizId, token);

        if (quiz is null)
            return Error.NotFound("Quiz.NotFound", $"Quiz {request.QuizId} not found");

        return quiz.ToDto();
    }
}
