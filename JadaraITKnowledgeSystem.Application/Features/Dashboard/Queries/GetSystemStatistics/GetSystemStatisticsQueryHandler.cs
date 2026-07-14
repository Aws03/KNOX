using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JadaraITKnowledgeSystem.Application.Features.Dashboard.Dtos;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JadaraITKnowledgeSystem.Application.Features.Dashboard.Queries.GetSystemStatistics;

public sealed class GetSystemStatisticsQueryHandler : IRequestHandler<GetSystemStatisticsQuery, Result<SystemStatisticsDto>>
{
    private readonly IApplicationDbContext _db;

    public GetSystemStatisticsQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result<SystemStatisticsDto>> Handle(GetSystemStatisticsQuery request, CancellationToken cancellationToken)
    {
        var utcNow = DateTime.UtcNow;
        var months = request.Months <= 0 ? 12 : request.Months;
        var startOfCurrentMonth = new DateTime(utcNow.Year, utcNow.Month, 1);
        var startDate = startOfCurrentMonth.AddMonths(-(months - 1)); // inclusive window: first day of start month

        // Totals (sequential to avoid concurrent DbContext operations)
        var totalUniversities = await _db.Universities.CountAsync(cancellationToken);
        var totalFaculties = await _db.Faculties.CountAsync(cancellationToken);
        var totalMajors = await _db.Majors.CountAsync(cancellationToken);
        var totalCourses = await _db.Courses.CountAsync(cancellationToken);
        var totalQuizzes = await _db.Quizzes.CountAsync(cancellationToken);
        var totalMaterials = await _db.CourseMaterials.CountAsync(cancellationToken);
        var totalUsers = await _db.Users.CountAsync(cancellationToken);

        // Helper to fill missing months
        static IReadOnlyList<SystemStatisticsDto.MonthlyCountDto> FillMonthlySeries(Dictionary<DateTime, int> map, DateTime startDate, DateTime endDate)
        {
            var series = new List<SystemStatisticsDto.MonthlyCountDto>();
            var cursor = new DateTime(startDate.Year, startDate.Month, 1);
            var lastMonth = new DateTime(endDate.Year, endDate.Month, 1);

            while (cursor <= lastMonth)
            {
                map.TryGetValue(cursor, out var c);
                series.Add(new SystemStatisticsDto.MonthlyCountDto { Month = DateOnly.FromDateTime(cursor), Count = c });
                cursor = cursor.AddMonths(1);
            }
            return series;
        }

        // Quizzes growth (group by Year/Month on server)
        var quizzesGrowthGroups = await _db.Quizzes
            .Where(q => q.CreatedAt >= startDate)
            .GroupBy(q => new { q.CreatedAt.Year, q.CreatedAt.Month })
            .Select(g => new { Year = g.Key.Year, Month = g.Key.Month, Count = g.Count() })
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        var quizzesMap = quizzesGrowthGroups.ToDictionary(
            x => new DateTime(x.Year, x.Month, 1),
            x => x.Count);
        var quizzesGrowth = FillMonthlySeries(quizzesMap, startDate, utcNow);

        // Materials growth (group by Year/Month on server)
        var materialsGrowthGroups = await _db.CourseMaterials
            .Where(m => m.CreatedAt >= startDate)
            .GroupBy(m => new { m.CreatedAt.Year, m.CreatedAt.Month })
            .Select(g => new { Year = g.Key.Year, Month = g.Key.Month, Count = g.Count() })
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        var materialsMap = materialsGrowthGroups.ToDictionary(
            x => new DateTime(x.Year, x.Month, 1),
            x => x.Count);
        var materialsGrowth = FillMonthlySeries(materialsMap, startDate, utcNow);

        // Users growth (group by Year/Month on server)
        var usersGrowthGroups = await _db.Users
            .Where(u => u.CreatedAt >= startDate)
            .GroupBy(u => new { u.CreatedAt.Year, u.CreatedAt.Month })
            .Select(g => new { Year = g.Key.Year, Month = g.Key.Month, Count = g.Count() })
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        var usersMap = usersGrowthGroups.ToDictionary(
            x => new DateTime(x.Year, x.Month, 1),
            x => x.Count);
        var usersGrowth = FillMonthlySeries(usersMap, startDate, utcNow);

        // Other statistics (sequential to avoid concurrent DbContext operations)
        var activeQuizAttemptsLast30Days = await _db.QuizAttempts
            .Where(a => a.CreatedAt >= utcNow.Date.AddDays(-30))
            .CountAsync(cancellationToken);

        var courseCount = await _db.Courses.CountAsync(cancellationToken);
        var materialsPerCourse = await _db.CourseMaterials
            .GroupBy(m => m.CourseId)
            .Select(g => g.Count())
            .ToListAsync(cancellationToken);
        var quizzesPerCourse = await _db.Quizzes
            .GroupBy(q => q.CourseId)
            .Select(g => g.Count())
            .ToListAsync(cancellationToken);

        var avgMaterialsPerCourse = courseCount == 0 ? 0 : materialsPerCourse.DefaultIfEmpty(0).Average();
        var avgQuizzesPerCourse = courseCount == 0 ? 0 : quizzesPerCourse.DefaultIfEmpty(0).Average();

        var dto = new SystemStatisticsDto
        {
            TotalUniversities = totalUniversities,
            TotalFaculties = totalFaculties,
            TotalMajors = totalMajors,
            TotalCourses = totalCourses,
            TotalQuizzes = totalQuizzes,
            TotalMaterials = totalMaterials,
            TotalUsers = totalUsers,
            QuizzesGrowth = quizzesGrowth,
            MaterialsGrowth = materialsGrowth,
            UsersGrowth = usersGrowth,
            ActiveQuizAttemptsLast30Days = activeQuizAttemptsLast30Days,
            AverageMaterialsPerCourse = avgMaterialsPerCourse,
            AverageQuizzesPerCourse = avgQuizzesPerCourse
        };

        return dto;
    }
}
