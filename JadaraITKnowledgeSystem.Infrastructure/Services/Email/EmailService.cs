using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Application.Common.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Infrastructure.Services.Email;

public class EmailService : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly string _accountId;
    private readonly string _apiKey;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public EmailService(IConfiguration configuration, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _accountId = configuration["AhaSend:AccountId"] 
            ?? throw new ArgumentNullException("AhaSend:AccountId", "AhaSend AccountId is not configured.");
        _apiKey = configuration["AhaSend:ApiKey"] 
            ?? throw new ArgumentNullException("AhaSend:ApiKey", "AhaSend ApiKey is not configured.");
        _fromEmail = configuration["AhaSend:FromEmail"] 
            ?? throw new ArgumentNullException("AhaSend:FromEmail", "AhaSend FromEmail is not configured.");
        _fromName = configuration["AhaSend:FromName"] ?? "UniHub";
    }

    public async Task<bool> SendEmailAsync(EmailRequest emailRequest)
    {
        // Validation: Either text_content or html_content required
        if (string.IsNullOrWhiteSpace(emailRequest.text_content) && string.IsNullOrWhiteSpace(emailRequest.html_content))
            throw new ArgumentException("Either text_content or html_content must be provided.");
        if (emailRequest.recipients is null || emailRequest.recipients.Count == 0)
            throw new ArgumentException("At least one recipient is required.");

        var request = new HttpRequestMessage(HttpMethod.Post, $"https://api.ahasend.com/v2/accounts/{_accountId}/messages")
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", _apiKey) },
            Content = new StringContent(
                JsonSerializer.Serialize(emailRequest, new JsonSerializerOptions 
                { 
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
                }), 
                Encoding.UTF8, 
                "application/json")
        };

        var response = await _httpClient.SendAsync(request);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Email send failed with status {response.StatusCode}: {errorContent}");
        }

        return true;
    }

    public EmailFrom GetDefaultFrom() => new EmailFrom { email = _fromEmail, name = _fromName };
}
