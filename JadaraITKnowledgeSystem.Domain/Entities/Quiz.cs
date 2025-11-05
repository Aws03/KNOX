using JadaraITKnowledgeSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Domain.Entities
{
    public class Quiz : AuditableEntity
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

        public Quiz(int courseId, int writerId, string title, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required");

            CourseId = courseId;
            WriterId = writerId;
            Title = title;
            Description = description;
            CreatedAt = DateTime.UtcNow;
        }

        public void AddQuestion(Question question)
        {
            if (question == null) throw new ArgumentNullException(nameof(question));
            _questions.Add(question);
        }

        public void AddReaction(UserReaction reaction)
        {
            if (_reactions.Any(r => r.UserId == reaction.UserId))
                throw new InvalidOperationException("User already reacted to this quiz");

            _reactions.Add(reaction);

            if (reaction.IsLike) Likes++;
            else Dislikes++;
        }
    }

}
