namespace JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Dtos;

public sealed record ChoiceDto
{
    public int Id { get; init; }
    public int QuestionId { get; init; }
    public string Text { get; init; } = String.Empty;
    public string? ImageUrl { get; init; }
    public bool IsCorrect { get; init; }
}
