using System;
using System.Collections.Generic;

namespace JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;

public sealed record QuizSummaryDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public int Likes { get; init; }
    public string WriterName { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
    public decimal? LastAttemptScore { get; init; }
}
