namespace JadaraITKnowledgeSystem.Application.Interfaces;

public interface ICurrentUserService
{
    /// <summary>The ASP.NET Identity user id (the JWT subject).</summary>
    int? UserId { get; }

    /// <summary>
    /// The id of the domain <c>Users</c> row. Identity and domain ids come from separate
    /// identity columns and are not guaranteed to match, so anything stored against a
    /// domain user (quizzes, enrollments, generation jobs) must use this, not <see cref="UserId"/>.
    /// </summary>
    int? DomainUserId { get; }

    string? Email { get; }
    IReadOnlyList<string> Roles { get; }
}
