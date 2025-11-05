namespace JadaraITKnowledgeSystem.Application.DTOs
{
    public class QuizDto
    {
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public int WriterId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
