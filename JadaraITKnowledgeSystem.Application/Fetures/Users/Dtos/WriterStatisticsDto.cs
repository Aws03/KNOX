namespace JadaraITKnowledgeSystem.Application.Fetures.Users.Dtos
{
    public class WriterStatisticsDto
    {
        public int TotalMaterials { get; set; }
        public int TotalQuizzes { get; set; }
        public int TotalQuizAttempts { get; set; }
        public int TotalQuizLikes { get; set; }
        public int TotalQuizDislikes { get; set; }
        public int TotalQuizQuestions { get; set; }
        public int TotalQuizChoices { get; set; }
        // Add more fields as needed for dashboard
    }
}
