using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Application.Common.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Infrastructure.Services.Email;

public class BrevoEmailService : IEmailService
{
    private const string SenderEmail = "aws.03.dev@gmail.com";
    private const string SenderName = "KNOX";

    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public BrevoEmailService(IConfiguration configuration, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Brevo:ApiKey"]
            ?? throw new ArgumentNullException("Brevo:ApiKey", "Brevo ApiKey is not configured.");
    }

    public async Task<bool> SendEmailAsync(EmailRequest emailRequest)
    {
        // Validation: Either text_content or html_content required
        if (string.IsNullOrWhiteSpace(emailRequest.text_content) && string.IsNullOrWhiteSpace(emailRequest.html_content))
            throw new ArgumentException("Either text_content or html_content must be provided.");
        if (emailRequest.recipients is null || emailRequest.recipients.Count == 0)
            throw new ArgumentException("At least one recipient is required.");

        var payload = new BrevoEmailRequest(
            sender: new BrevoSender(SenderName, SenderEmail),
            to: emailRequest.recipients.Select(r => new BrevoRecipient(r.email, r.name)).ToList(),
            subject: emailRequest.subject,
            htmlContent: emailRequest.html_content,
            textContent: emailRequest.text_content);

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/smtp/email")
        {
            Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
        };
        request.Headers.Add("api-key", _apiKey);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Email send failed with status {response.StatusCode}: {errorContent}");
        }

        return true;
    }

    public EmailFrom GetDefaultFrom() => new EmailFrom { email = SenderEmail, name = SenderName };

    private sealed record BrevoSender(string name, string email);
    private sealed record BrevoRecipient(string email, string? name);
    private sealed record BrevoEmailRequest(BrevoSender sender, List<BrevoRecipient> to, string subject, string? htmlContent, string? textContent);
}
