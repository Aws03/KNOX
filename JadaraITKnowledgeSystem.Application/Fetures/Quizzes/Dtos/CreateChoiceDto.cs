namespace JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Dtos
{
    public record CreateChoiceDto
    {
        public string Text { get; init; }
        public bool IsCorrect { get; init; }
    }
}
