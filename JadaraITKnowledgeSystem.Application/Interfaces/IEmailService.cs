namespace JadaraITKnowledgeSystem.Application.Interfaces;

public interface IEmailService
{
    /// <summary>Sends the message or throws if the provider rejects it.</summary>
    Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
}

public sealed record EmailMessage(string ToEmail, string? ToName, string Subject, string HtmlBody, string TextBody);
