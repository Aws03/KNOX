using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JadaraITKnowledgeSystem.Domain.Common;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Users;

namespace JadaraITKnowledgeSystem.Domain.Courses.Entities;

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

    // Grading is intentionally not modelled yet: universities use different grading
    // schemes (A/A+/B..., percentages), so it needs its own design before it lands here.

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
        return Result.Success;
    }
}
