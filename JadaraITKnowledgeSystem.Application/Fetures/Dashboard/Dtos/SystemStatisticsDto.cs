using System;
using System.Collections.Generic;

namespace JadaraITKnowledgeSystem.Application.Fetures.Dashboard.Dtos;

public sealed class SystemStatisticsDto
{
    // Totals
    public int TotalUniversities { get; init; }
    public int TotalFaculties { get; init; }
    public int TotalMajors { get; init; }
    public int TotalCourses { get; init; }
    public int TotalQuizzes { get; init; }
    public int TotalMaterials { get; init; }
    public int TotalUsers { get; init; }

    // Growth metrics (per month counts)
    public IReadOnlyList<MonthlyCountDto> QuizzesGrowth { get; init; } = Array.Empty<MonthlyCountDto>();
    public IReadOnlyList<MonthlyCountDto> MaterialsGrowth { get; init; } = Array.Empty<MonthlyCountDto>();
    public IReadOnlyList<MonthlyCountDto> UsersGrowth { get; init; } = Array.Empty<MonthlyCountDto>();

    // Other useful statistics
    public int ActiveQuizAttemptsLast30Days { get; init; }
    public double AverageMaterialsPerCourse { get; init; }
    public double AverageQuizzesPerCourse { get; init; }

    public sealed class MonthlyCountDto
    {
        public DateOnly Month { get; init; }
        public int Count { get; init; }
    }
}
