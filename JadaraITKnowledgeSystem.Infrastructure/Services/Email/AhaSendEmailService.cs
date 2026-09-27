using System.Net.Http.Headers;
using System.Net.Http.Json;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace JadaraITKnowledgeSystem.Infrastructure.Services.Email;

public sealed class AhaSendEmailService(HttpClient httpClient, IOptions<AhaSendOptions> options) : IEmailService
{
    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        var settings = options.Value;
        using var request = new HttpRequestMessage(HttpMethod.Post, $"https://api.ahasend.com/v2/accounts/{settings.AccountId}/messages")
        {
            Content = JsonContent.Create(new
            {
                from = new { email = settings.FromEmail, name = settings.FromName },
                recipients = new[] { new { email = message.ToEmail, name = message.ToName } },
                subject = message.Subject,
                text_content = message.TextBody,
                html_content = message.HtmlBody
            })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", settings.ApiKey);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        await BrevoEmailService.EnsureSuccessAsync(response, "AhaSend", cancellationToken);
    }
}
