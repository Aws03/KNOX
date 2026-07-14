namespace JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;

public record CreateChoiceDto
{
    public string Text { get; init; } = String.Empty;
    public string? ImageUrl { get; set; }
    public bool IsCorrect { get; init; }
}
