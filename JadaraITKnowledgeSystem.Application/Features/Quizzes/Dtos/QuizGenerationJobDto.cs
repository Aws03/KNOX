using System;
using System.Collections.Generic;
using JadaraITKnowledgeSystem.Domain.Quizzes.Enums;

namespace JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;

public sealed record QuizGenerationJobDto
{
    public int Id { get; init; }
    public int MaterialId { get; init; }
    public string MaterialTitle { get; init; }
    public int CourseId { get; init; }
    public string CourseName { get; init; }
    public QuizGenerationStatus Status { get; init; }
    public int GeneratedQuizCount { get; init; }
    public List<int> GeneratedQuizIds { get; init; }
    public string? ErrorMessage { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public QuizGenerationOptionsDto Options { get; init; }
}
