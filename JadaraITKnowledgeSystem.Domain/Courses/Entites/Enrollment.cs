using JadaraITKnowledgeSystem.Domain.Common;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JadaraITKnowledgeSystem.Domain.Courses.Entites;

/// <summary>
/// Represents a user's enrollment in a course.
/// Tracks enrollment status, completion, and optional grade.
/// </summary>
public sealed class Enrollment : AuditableEntity
{
    [ForeignKey(nameof(User))]
    public int UserId { get; private set; }
    public User User { get; private set; } = null!;

    [ForeignKey(nameof(Course))]
    public int CourseId { get; private set; }
    public Course Course { get; private set; } = null!;

    /// <summary>
    /// Indicates whether the user has completed and passed the course.
    /// </summary>
    public bool IsFinished { get; private set; }

    /// <summary>
    /// The date and time when the user finished the course.
    /// Null if the course is not yet finished.
    /// </summary>
    public DateTimeOffset? FinishedAt { get; private set; }

    // TODO: Grade functionality is temporarily disabled.
    // Universities may have different grading systems (A, A+, B, etc.)
    // This needs to be redesigned to support flexible grading systems.
    // /// <summary>
    // /// Optional grade assigned after course completion.
    // /// Value should be between 0 and 100 (percentage) or null if not graded.
    // /// </summary>
    // [Range(0, 100)]
    // public decimal? Grade { get; private set; }

    /// <summary>
    /// Optional notes or feedback about the enrollment.
    /// </summary>
    [MaxLength(500)]
    public string? Notes { get; private set; }

    private Enrollment() { }

    private Enrollment(int userId, int courseId)
    {
        UserId = userId;
        CourseId = courseId;
        IsFinished = false;
        FinishedAt = null;
        // Grade = null;
    }

    /// <summary>
    /// Creates a new enrollment for a user in a course.
    /// </summary>
    public static Result<Enrollment> Create(int userId, int courseId)
    {
        if (userId <= 0)
            return Error.Validation("Enrollment.UserId.Invalid", "UserId must be a positive integer.");

        if (courseId <= 0)
            return Error.Validation("Enrollment.CourseId.Invalid", "CourseId must be a positive integer.");

        return new Enrollment(userId, courseId);
    }

    /// <summary>
    /// Marks the enrollment as completed/finished.
    /// </summary>
    public Result<Success> Complete()
    {
        if (IsFinished)
            return Error.Conflict("Enrollment.AlreadyFinished", "This enrollment is already marked as finished.");

        IsFinished = true;
        FinishedAt = DateTimeOffset.UtcNow;
        return Result.Success;
    }

    // TODO: Grade functionality is temporarily disabled.
    // Universities may have different grading systems (A, A+, B, etc.)
    // This needs to be redesigned to support flexible grading systems.
    // /// <summary>
    // /// Sets the grade for a finished course.
    // /// </summary>
    // /// <param name="grade">Grade value between 0 and 100.</param>
    // public Result<Success> SetGrade(decimal grade)
    // {
    //     if (!IsFinished)
    //         return Error.Validation("Enrollment.NotFinished", "Cannot set grade for a course that is not finished.");
    // 
    //     if (grade < 0 || grade > 100)
    //         return Error.Validation("Enrollment.Grade.Invalid", "Grade must be between 0 and 100.");
    // 
    //     Grade = grade;
    //     return Result.Success;
    // }

    // TODO: Grade functionality is temporarily disabled.
    // Universities may have different grading systems (A, A+, B, etc.)
    // This needs to be redesigned to support flexible grading systems.
    // /// <summary>
    // /// Completes the course and sets the grade in one operation.
    // /// </summary>
    // public Result<Success> CompleteWithGrade(decimal grade)
    // {
    //     if (IsFinished)
    //         return Error.Conflict("Enrollment.AlreadyFinished", "This enrollment is already marked as finished.");
    // 
    //     if (grade < 0 || grade > 100)
    //         return Error.Validation("Enrollment.Grade.Invalid", "Grade must be between 0 and 100.");
    // 
    //     IsFinished = true;
    //     FinishedAt = DateTimeOffset.UtcNow;
    //     Grade = grade;
    //     return Result.Success;
    // }

    /// <summary>
    /// Updates the notes for this enrollment.
    /// </summary>
    public Result<Success> UpdateNotes(string? notes)
    {
        if (notes != null && notes.Length > 500)
            return Error.Validation("Enrollment.Notes.TooLong", "Notes cannot exceed 500 characters.");

        Notes = notes?.Trim();
        return Result.Success;
    }

    /// <summary>
    /// Resets the completion status (e.g., for re-taking the course).
    /// </summary>
    public Result<Success> ResetCompletion()
    {
        IsFinished = false;
        FinishedAt = null;
        // Grade = null;
        return Result.Success;
    }
}
