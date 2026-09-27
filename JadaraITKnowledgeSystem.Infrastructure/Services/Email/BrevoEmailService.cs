using System.Net.Http.Json;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace JadaraITKnowledgeSystem.Infrastructure.Services.Email;

public sealed class BrevoEmailService(HttpClient httpClient, IOptions<BrevoOptions> options) : IEmailService
{
    private const string Endpoint = "https://api.brevo.com/v3/smtp/email";

    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        var settings = options.Value;
        using var request = new HttpRequestMessage(HttpMethod.Post, Endpoint)
        {
            Content = JsonContent.Create(new
            {
                sender = new { name = settings.FromName, email = settings.FromEmail },
                to = new[] { new { email = message.ToEmail, name = message.ToName } },
                subject = message.Subject,
                htmlContent = message.HtmlBody,
                textContent = message.TextBody
            })
        };
        request.Headers.Add("api-key", settings.ApiKey);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        await EnsureSuccessAsync(response, "Brevo", cancellationToken);
    }

    internal static async Task EnsureSuccessAsync(HttpResponseMessage response, string provider, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
            return;

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        throw new HttpRequestException($"{provider} rejected the email with status {(int)response.StatusCode}: {body}");
    }
}
