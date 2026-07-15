using System.Threading;
using System.Threading.Tasks;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JadaraITKnowledgeSystem.Application.Features.Quizzes.Queries.GetQuizGenerationJobStatus;

public sealed class GetQuizGenerationJobStatusQueryHandler 
    : IRequestHandler<GetQuizGenerationJobStatusQuery, Result<QuizGenerationJobDto>>
{
    private readonly IApplicationDbContext _context;

    public GetQuizGenerationJobStatusQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<QuizGenerationJobDto>> Handle(
        GetQuizGenerationJobStatusQuery request,
        CancellationToken cancellationToken)
    {
        var job = await _context.QuizGenerationJobs
            .Include(j => j.Material)
            .Include(j => j.Course)
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == request.JobId, cancellationToken);

        if (job == null)
        {
            return Error.NotFound(
                "QuizGenerationJob.NotFound",
                $"Quiz generation job with ID {request.JobId} not found");
        }

        return job.ToDto();
    }
}
