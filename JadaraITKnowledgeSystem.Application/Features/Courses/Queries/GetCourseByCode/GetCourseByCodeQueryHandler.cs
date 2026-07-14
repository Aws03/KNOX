using JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Courses.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Queries.GetCourseByCode;

public sealed class GetCourseByCodeQueryHandler : IRequestHandler<GetCourseByCodeQuery, Result<CourseDto>>
{
    private readonly IApplicationDbContext _context;
    public GetCourseByCodeQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CourseDto>> Handle(GetCourseByCodeQuery request, CancellationToken cancellationToken)
    {
        var course = await _context.Courses
            .AsNoTracking()
            .Where(c => c.CourseCode == request.CourseCode)
            .Select(c => c.ToDto())
            .FirstOrDefaultAsync(cancellationToken);

        if (course == null)
            return Error.NotFound("Course.NotFound", $"Course with code '{request.CourseCode}' not found.");

        return course;
    }
}
