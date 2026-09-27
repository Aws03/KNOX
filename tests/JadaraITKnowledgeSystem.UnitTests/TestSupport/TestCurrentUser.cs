using JadaraITKnowledgeSystem.Application.Interfaces;

namespace JadaraITKnowledgeSystem.UnitTests.TestSupport;

public sealed class TestCurrentUser : ICurrentUserService
{
    public int? UserId { get; set; }
    public int? DomainUserId { get; set; }
    public string? Email { get; set; }
    public IReadOnlyList<string> Roles { get; set; } = [];

    public static TestCurrentUser Anonymous() => new();

    public static TestCurrentUser InRole(string role, int identityUserId = 1, int? domainUserId = null, string email = "caller@test.local") =>
        new() { UserId = identityUserId, DomainUserId = domainUserId ?? identityUserId, Email = email, Roles = [role] };
}
