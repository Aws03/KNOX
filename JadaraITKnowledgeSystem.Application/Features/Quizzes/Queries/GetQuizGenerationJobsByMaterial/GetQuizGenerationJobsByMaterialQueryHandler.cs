using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JadaraITKnowledgeSystem.Application.Features.Quizzes.Queries.GetQuizGenerationJobsByMaterial;

public sealed class GetQuizGenerationJobsByMaterialQueryHandler 
    : IRequestHandler<GetQuizGenerationJobsByMaterialQuery, Result<List<QuizGenerationJobDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetQuizGenerationJobsByMaterialQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<QuizGenerationJobDto>>> Handle(
        GetQuizGenerationJobsByMaterialQuery request,
        CancellationToken cancellationToken)
    {
        var jobs = await _context.QuizGenerationJobs
            .Where(j => j.MaterialId == request.MaterialId)
            .Include(j => j.Material)
            .Include(j => j.Course)
            .OrderByDescending(j => j.CreatedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return jobs.Select(j => j.ToDto()).ToList();
    }
}
