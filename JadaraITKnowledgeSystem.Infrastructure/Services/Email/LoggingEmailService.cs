using JadaraITKnowledgeSystem.Application.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Infrastructure.Services.Email;

/// <summary>
/// Used when no email provider is configured. Nothing is delivered; in Development the
/// plain-text body (which carries OTP codes) is logged so the flows can be exercised locally.
/// </summary>
public sealed class LoggingEmailService(IHostEnvironment environment, ILogger<LoggingEmailService> logger) : IEmailService
{
    public Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        if (environment.IsDevelopment())
        {
            logger.LogInformation("Email to {Recipient} (not delivered, no provider configured): {Subject}\n{Body}",
                message.ToEmail, message.Subject, message.TextBody);
        }
        else
        {
            logger.LogWarning("No email provider is configured; dropped email '{Subject}'", message.Subject);
        }

        return Task.CompletedTask;
    }
}
