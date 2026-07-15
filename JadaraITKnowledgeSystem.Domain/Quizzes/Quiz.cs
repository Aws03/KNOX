using JadaraITKnowledgeSystem.Domain.Common;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Courses;
using JadaraITKnowledgeSystem.Domain.Courses.Entites;
using JadaraITKnowledgeSystem.Domain.Quizzes.Entites;
using JadaraITKnowledgeSystem.Domain.Quizzes.Enums;
using JadaraITKnowledgeSystem.Domain.Quizzes.Errors;
using JadaraITKnowledgeSystem.Domain.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace JadaraITKnowledgeSystem.Domain.Quizzes
{
    public sealed class Quiz : AuditableEntity
    {
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

        public QuizSource Source { get; private set; } = QuizSource.Manual;
        
        [ForeignKey(nameof(SourceMaterial))]
        public int? SourceMaterialId { get; private set; }
        public CourseMaterial? SourceMaterial { get; private set; }
        
        public int? PartNumber { get; private set; }
        public int? TotalParts { get; private set; }

        private readonly List<Question> _questions = new();
        public IReadOnlyCollection<Question> Questions => _questions.AsReadOnly();

        private readonly List<UserReaction> _reactions = new();
        public IReadOnlyCollection<UserReaction> Reactions => _reactions.AsReadOnly();
        private readonly List<QuizAttempt> _attempts = new();
        public IReadOnlyCollection<QuizAttempt> Attempts => _attempts.AsReadOnly();

        private readonly List<string> _tags = new();
        public IReadOnlyCollection<string> Tags => _tags.AsReadOnly();

        public void AddOrUpdateAttempt(QuizAttempt attempt)
        {
            var existing = _attempts.FirstOrDefault(a => a.UserId == attempt.UserId);
            if (existing != null)
            {
                existing.UpdateScore(attempt.Score);
            }
            else
            {
                _attempts.Add(attempt);
            }
        }

        private Quiz() { }

        private Quiz(
            int courseId, 
            int writerId, 
            string title, 
            string? description = null,
            QuizSource source = QuizSource.Manual,
            int? sourceMaterialId = null,
            int? partNumber = null,
            int? totalParts = null)
        {
            CourseId = courseId;
            WriterId = writerId;
            Title = title;
            Description = description;
            CreatedAt = DateTime.UtcNow;
            Source = source;
            SourceMaterialId = sourceMaterialId;
            PartNumber = partNumber;
            TotalParts = totalParts;
        }

        public static Result<Quiz> Create(
            int courseId, 
            int writerId, 
            string title, 
            string? description = null, 
            IEnumerable<string>? tags = null,
            QuizSource source = QuizSource.Manual,
            int? sourceMaterialId = null,
            int? partNumber = null,
            int? totalParts = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                return QuizErrors.TitleRequired;

            var quiz = new Quiz(courseId, writerId, title, description, source, sourceMaterialId, partNumber, totalParts);
            var tagResult = quiz.UpdateTags(tags);
            if (tagResult.IsError)
                return tagResult.Errors;

            return quiz;
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
            var existingReaction = _reactions.FirstOrDefault(r => r.UserId == reaction.UserId);

            if (existingReaction != null)
            {
                if (existingReaction.ReactionType == reaction.ReactionType)
                {
                    return QuizErrors.ConflictReaction;
                }

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
                if (reaction.ReactionType == ReactionType.Like)
                    Likes++;
                else
                    Dislikes++;
            }

            _reactions.Add(reaction);

            return Result.Success;
        }

        public Result<Success> UpdateTags(IEnumerable<string>? tags)
        {
            _tags.Clear();
            if (tags is null)
                return Result.Success;

            var normalized = new List<string>();
            foreach (var tag in tags)
            {
                if (string.IsNullOrWhiteSpace(tag))
                    return Error.Validation("Quiz.Tag.Invalid", "Tag cannot be empty.");

                var trimmed = tag.Trim();
                if (trimmed.Length > 50)
                    return Error.Validation("Quiz.Tag.TooLong", "Tag cannot exceed 50 characters.");

                if (normalized.Any(t => string.Equals(t, trimmed, StringComparison.OrdinalIgnoreCase)))
                    continue;

                if (normalized.Count >= 10)
                    return Error.Validation("Quiz.Tag.Limit", "A quiz can have at most 10 tags.");

                normalized.Add(trimmed);
            }

            _tags.AddRange(normalized);
            return Result.Success;
        }
    }
}
