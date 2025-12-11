using System;
using System.Collections.Generic;

namespace JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Dtos;

public sealed record QuizSummaryDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public int Likes { get; init; }
    public string WriterName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public decimal? LastAttemptScore { get; init; }
    public List<string> Tags { get; init; } = new();
}
