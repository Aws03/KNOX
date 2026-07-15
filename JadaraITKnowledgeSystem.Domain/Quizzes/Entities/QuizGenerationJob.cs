using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using JadaraITKnowledgeSystem.Domain.Common;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Courses;
using JadaraITKnowledgeSystem.Domain.Courses.Entites;
using JadaraITKnowledgeSystem.Domain.Quizzes.Enums;
using JadaraITKnowledgeSystem.Domain.Users;

namespace JadaraITKnowledgeSystem.Domain.Quizzes.Entities;

public sealed class QuizGenerationJob : AuditableEntity
{
    [ForeignKey(nameof(Material))]
    public int MaterialId { get; private set; }
    public CourseMaterial Material { get; private set; }

    [ForeignKey(nameof(Course))]
    public int CourseId { get; private set; }
    public Course Course { get; private set; }

    [ForeignKey(nameof(RequestedByUser))]
    public int RequestedByUserId { get; private set; }
    public User RequestedByUser { get; private set; }

    public QuizGenerationStatus Status { get; private set; }

    [MaxLength(1000)]
    public string? ErrorMessage { get; private set; }

    public int GeneratedQuizCount { get; private set; }

    [MaxLength(500)]
    public string? GeneratedQuizIdsJson { get; private set; }

    [MaxLength(1000)]
    public string OptionsJson { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    private QuizGenerationJob() { }

    private QuizGenerationJob(
        int materialId,
        int courseId,
        int requestedByUserId,
        QuizGenerationOptions options)
    {
        MaterialId = materialId;
        CourseId = courseId;
        RequestedByUserId = requestedByUserId;
        Status = QuizGenerationStatus.Pending;
        GeneratedQuizCount = 0;
        OptionsJson = JsonSerializer.Serialize(options);
    }

    public static Result<QuizGenerationJob> Create(
        int materialId,
        int courseId,
        int requestedByUserId,
        QuizGenerationOptions options)
    {
        if (materialId <= 0)
            return Error.Validation("QuizGenerationJob.MaterialId.Invalid", "Material ID must be positive");

        if (courseId <= 0)
            return Error.Validation("QuizGenerationJob.CourseId.Invalid", "Course ID must be positive");

        if (requestedByUserId <= 0)
            return Error.Validation("QuizGenerationJob.UserId.Invalid", "User ID must be positive");

        if (options == null)
            return Error.Validation("QuizGenerationJob.Options.Required", "Options are required");

        return new QuizGenerationJob(materialId, courseId, requestedByUserId, options);
    }

    public void UpdateStatus(QuizGenerationStatus status)
    {
        Status = status;
    }

    public void MarkFailed(string errorMessage)
    {
        Status = QuizGenerationStatus.Failed;
        ErrorMessage = errorMessage?.Length > 1000 
            ? errorMessage.Substring(0, 997) + "..." 
            : errorMessage;
        CompletedAt = DateTime.UtcNow;
    }

    public void MarkCompleted(int generatedQuizCount)
    {
        Status = QuizGenerationStatus.Completed;
        GeneratedQuizCount = generatedQuizCount;
        CompletedAt = DateTime.UtcNow;
    }

    public void AddGeneratedQuizId(int quizId)
    {
        var quizIds = GetGeneratedQuizIds();
        quizIds.Add(quizId);
        GeneratedQuizIdsJson = JsonSerializer.Serialize(quizIds);
    }

    public List<int> GetGeneratedQuizIds()
    {
        if (string.IsNullOrEmpty(GeneratedQuizIdsJson))
            return new List<int>();

        try
        {
            return JsonSerializer.Deserialize<List<int>>(GeneratedQuizIdsJson) ?? new List<int>();
        }
        catch
        {
            return new List<int>();
        }
    }

    public QuizGenerationOptions GetOptions()
    {
        try
        {
            return JsonSerializer.Deserialize<QuizGenerationOptions>(OptionsJson) 
                ?? new QuizGenerationOptions();
        }
        catch
        {
            return new QuizGenerationOptions();
        }
    }
}

public sealed class QuizGenerationOptions
{
    public int QuestionsPerQuiz { get; set; } = 8;
    public string Difficulty { get; set; } = "Medium";
    public int MaxQuizzes { get; set; } = 5;
    public bool AutoPublish { get; set; } = false;
}
