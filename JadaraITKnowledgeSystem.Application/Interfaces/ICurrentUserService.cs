namespace JadaraITKnowledgeSystem.Application.Interfaces;

public interface ICurrentUserService
{
    int? UserId { get; }
    string? Email { get; }
    IReadOnlyList<string> Roles { get; }
}
