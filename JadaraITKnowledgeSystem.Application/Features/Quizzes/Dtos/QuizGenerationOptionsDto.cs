namespace JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;

public sealed record QuizGenerationOptionsDto
{
    public int QuestionsPerQuiz { get; init; } = 8;
    public string Difficulty { get; init; } = "Medium";
    public int MaxQuizzes { get; init; } = 5;
    public bool AutoPublish { get; init; } = false;
}
