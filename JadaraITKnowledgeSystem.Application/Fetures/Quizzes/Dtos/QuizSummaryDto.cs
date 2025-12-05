namespace JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Dtos;

public sealed record QuizSummaryDto
{
    public int Id { get; init; }
    public string Title { get; init; } = String.Empty;
    public int Likes { get; init; }
    public string WriterName { get; init; } = String.Empty;
    public DateTime CreatedAt { get; init; }
    // Nullable last attempt score for a specific user if requested
    public decimal? LastAttemptScore { get; init; }
}
