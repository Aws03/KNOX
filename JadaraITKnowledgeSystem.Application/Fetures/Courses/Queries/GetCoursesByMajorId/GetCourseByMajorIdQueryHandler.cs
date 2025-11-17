using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Fetures.Courses.Dtos;
using JadaraITKnowledgeSystem.Application.Fetures.Courses.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Queries.GetCoursesByMajorId
{
    public class GetCoursesByMajorIdQueryHandler
        (IApplicationDbContext context, ILogger<GetCoursesByMajorIdQueryHandler> logger)
        : IRequestHandler<GetCoursesByMajorIdQuery, Result<PaginatedList<CourseDto>>>
    {
        private readonly ILogger<GetCoursesByMajorIdQueryHandler> _logger = logger;
        private readonly IApplicationDbContext _context = context;

        public async Task<Result<PaginatedList<CourseDto>>> Handle(
            GetCoursesByMajorIdQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Retrieving courses for MajorId {MajorId}, Page {Page}, PageSize {PageSize}",
                request.MajorId, request.PageNumber, request.PageSize);

            // Query the courses associated with this major via mapping
            var query = _context.MajorCourses
                .AsNoTracking()
                .Where(mc => mc.MajorId == request.MajorId)
                .Include(mc => mc.Course)              
                    .ThenInclude(c => c.Requirements)  
                .Select(mc => mc.Course);             


            // Project to DTO using your mapper
            var coursesQuery = query.Select(c => c.ToDto());

            var paginated = await PaginatedList<CourseDto>.CreateAsync(
                coursesQuery,
                request.PageNumber,
                request.PageSize,
                cancellationToken);

            _logger.LogInformation(
                "Retrieved {Count} courses for MajorId {MajorId} on Page {Page}",
                paginated.Items.Count, request.MajorId, request.PageNumber);

            return paginated;
        }
    }
}
