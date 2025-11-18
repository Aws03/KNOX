using JadaraITKnowledgeSystem.Domain.Common;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Courses;
using JadaraITKnowledgeSystem.Domain.Quizzes.Entites;
using JadaraITKnowledgeSystem.Domain.Quizzes.Enums;
using JadaraITKnowledgeSystem.Domain.Quizzes.Errors;
using JadaraITKnowledgeSystem.Domain.Users;
using System.ComponentModel.DataAnnotations.Schema;


namespace JadaraITKnowledgeSystem.Domain.Quizzes
{
    public sealed class Quiz : AuditableEntity
    {
        //[Key]
        //public int QuizId { get; private set; }

        [ForeignKey(nameof(Course))]
        public int CourseId { get; private set; }
        public Course Course { get; private set; }

        [ForeignKey(nameof(User))]
        public int WriterId { get; private set; }
        public User Writer { get; private set; }

        public string Title { get; private set; }
        public string? Description { get; private set; }
        public int Likes { get; private set; }
        public int Dislikes { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private readonly List<Question> _questions = new();
        public IReadOnlyCollection<Question> Questions => _questions.AsReadOnly();

        private readonly List<UserReaction> _reactions = new();
        public IReadOnlyCollection<UserReaction> Reactions => _reactions.AsReadOnly();
        private readonly List<QuizAttempt> _attempts = new();
        public IReadOnlyCollection<QuizAttempt> Attempts => _attempts.AsReadOnly();

        public void AddOrUpdateAttempt(QuizAttempt attempt)
        {
            var existing = _attempts.FirstOrDefault(a => a.UserId == attempt.UserId);
            if (existing != null)
            {
                existing.UpdateScore(attempt.Score); // update last result
            }
            else
            {
                _attempts.Add(attempt);
            }
        }

        private Quiz() { }

        private Quiz(int courseId, int writerId, string title, string? description = null)
        {
            CourseId = courseId;
            WriterId = writerId;
            Title = title;
            Description = description;
            CreatedAt = DateTime.UtcNow;
        }

        public static Result<Quiz> Create(int courseId, int writerId, string title, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                return QuizErrors.TitleRequired;
            return new Quiz(courseId, writerId, title, description);
        }

        public Result<Success> AddQuestion(Question question)
        {
            if (question == null)
                return QuizErrors.QuestionRequired; 

            _questions.Add(question);

            return Result.Success;
        }

        public Result<Success> AddReaction(UserReaction reaction)
        {
            // Check if the same reaction already exists
            var existingReaction = _reactions.FirstOrDefault(r => r.UserId == reaction.UserId);

            if (existingReaction != null)
            {
                if (existingReaction.ReactionType == reaction.ReactionType)
                {
                    // Same reaction exists → conflict
                    return QuizErrors.ConflictReaction;
                }

                // Toggle reaction: remove previous reaction counts
                if (existingReaction.ReactionType == ReactionType.Like)
                {
                    Likes--;
                    Dislikes++;
                }
                else
                {
                    Likes++;
                    Dislikes--;
                }

                _reactions.Remove(existingReaction);
            }
            else
            {
                // New reaction, increment counters
                if (reaction.ReactionType == ReactionType.Like)
                    Likes++;
                else
                    Dislikes++;
            }

            // Add the new reaction
            _reactions.Add(reaction);

            return Result.Success;
        }

    }

}
