namespace JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Dtos;

public sealed record QuizDto
{
    public int Id { get; init; }
    public int CourseId { get; init; }
    public int WriterId { get; init; }
    public string? WriterName { get; init; } = "Unknown";
    public string Title { get; init; } = String.Empty;
    public string? Description { get; init; }
    public int Likes { get; init; }
    public int Dislikes { get; init; }
    public DateTime CreatedAt { get; init; }
    public List<QuestionDto> Questions { get; init; } = new();
}
