using System.Text;
using System.Text.Json;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Quizzes.Enums;
using JadaraITKnowledgeSystem.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JadaraITKnowledgeSystem.Infrastructure.Services.AI;

/// <summary>
/// Calls the OpenAI chat completions REST API (via a typed HttpClient) to generate quizzes from text.
/// </summary>
public sealed class OpenAIService(HttpClient httpClient, IOptions<OpenAIOptions> options, ILogger<OpenAIService> logger) : IOpenAIService
{
    private readonly ILogger<OpenAIService> _logger = logger;

    public async Task<Result<GeneratedQuizDto>> GenerateQuizFromTextAsync(
        GenerateQuizRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!options.Value.IsConfigured)
            return Error.Failure("OpenAI.NotConfigured", "AI quiz generation is not configured (OpenAI:ApiKey is missing).");

        try
        {
            _logger.LogInformation(
                "Generating quiz from text. QuestionCount={QuestionCount}, Difficulty={Difficulty}",
                request.QuestionCount, request.Difficulty);

            var prompt = BuildQuizGenerationPrompt(request);
            var response = await CallOpenAIAsync(prompt, cancellationToken);

            if (response.IsError)
                return response.Errors;

            var generatedText = response.Value;

            // Parse JSON response
            var quizDto = ParseQuizResponse(generatedText);
            if (quizDto == null)
            {
                _logger.LogWarning("Failed to parse OpenAI response as valid quiz JSON");
                return Error.Failure("OpenAI.ParseError", "Failed to parse AI response into quiz format");
            }

            _logger.LogInformation(
                "Quiz generated successfully. Questions={QuestionCount}",
                quizDto.Questions?.Count ?? 0);

            return quizDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating quiz from text");
            return Error.Failure("OpenAI.GenerationFailed", $"Failed to generate quiz: {ex.Message}");
        }
    }

    private async Task<Result<string>> CallOpenAIAsync(string prompt, CancellationToken cancellationToken)
    {
        try
        {
            var settings = options.Value;
            var requestBody = new
            {
                model = settings.Model,
                messages = new[]
                {
                    new { role = "system", content = GetSystemPrompt() },
                    new { role = "user", content = prompt }
                },
                max_tokens = settings.MaxTokens,
                temperature = settings.Temperature
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, settings.Endpoint)
            {
                Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", settings.ApiKey);

            using var response = await httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("OpenAI API error: {StatusCode} - {Error}", response.StatusCode, errorContent);
                return Error.Failure("OpenAI.APIError", $"OpenAI API returned {response.StatusCode}");
            }

            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
            var responseObj = JsonSerializer.Deserialize<JsonElement>(responseJson);

            var messageContent = responseObj
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return messageContent ?? string.Empty;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling OpenAI API");
            return Error.Failure("OpenAI.NetworkError", "Network error communicating with OpenAI API");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling OpenAI API");
            return Error.Failure("OpenAI.Error", $"Error calling OpenAI: {ex.Message}");
        }
    }

    private static string GetSystemPrompt()
    {
        return @"You are an expert educational quiz generator. Your task is to create high-quality, 
educational multiple-choice quizzes based on provided content. Always return responses in valid JSON format.
Be precise, educational, and ensure questions are clear and unambiguous.";
    }

    private static string BuildQuizGenerationPrompt(GenerateQuizRequest request)
    {
        var partInfo = request.TotalChunks > 1
            ? $"This is part {request.ChunkIndex + 1} of {request.TotalChunks} from a larger document."
            : "This is standalone content.";

        return $@"
Based on the following educational content, generate a quiz in JSON format.

{partInfo}

REQUIREMENTS:
- Create exactly {request.QuestionCount} multiple-choice questions
- Difficulty level: {request.Difficulty}
- Each question must have exactly 4 choices (labeled A, B, C, D)
- Only ONE choice should be correct
- Questions should cover different aspects of the content
- Include a brief topic summary (max 100 chars)
- Suggest 3-5 relevant tags

RESPONSE FORMAT (strict JSON):
{{
  ""topic"": ""Brief topic description"",
  ""title"": ""Suggested quiz title"",
  ""description"": ""2-3 sentence quiz description (max 500 chars)"",
  ""suggestedTags"": [""tag1"", ""tag2"", ""tag3""],
  ""questions"": [
    {{
      ""text"": ""Question text here?"",
      ""type"": 1,
      ""choices"": [
        {{""text"": ""Choice A"", ""isCorrect"": false}},
        {{""text"": ""Choice B"", ""isCorrect"": true}},
        {{""text"": ""Choice C"", ""isCorrect"": false}},
        {{""text"": ""Choice D"", ""isCorrect"": false}}
      ]
    }}
  ]
}}

EDUCATIONAL CONTENT:
{request.Text}

Generate the quiz now as valid JSON:
";
    }

    private GeneratedQuizDto? ParseQuizResponse(string jsonResponse)
    {
        try
        {
            // Clean the response (remove markdown code blocks if present)
            jsonResponse = jsonResponse.Trim();
            if (jsonResponse.StartsWith("```json"))
            {
                jsonResponse = jsonResponse.Substring(7);
            }
            if (jsonResponse.StartsWith("```"))
            {
                jsonResponse = jsonResponse.Substring(3);
            }
            if (jsonResponse.EndsWith("```"))
            {
                jsonResponse = jsonResponse.Substring(0, jsonResponse.Length - 3);
            }
            jsonResponse = jsonResponse.Trim();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var quizData = JsonSerializer.Deserialize<GeneratedQuizDto>(jsonResponse, options);

            // Validate and set defaults
            if (quizData != null)
            {
                if (string.IsNullOrWhiteSpace(quizData.Topic)) quizData.Topic = "General Quiz";
                if (string.IsNullOrWhiteSpace(quizData.Title)) quizData.Title = "Generated Quiz";
                if (string.IsNullOrWhiteSpace(quizData.Description)) quizData.Description = "Test your knowledge with this quiz.";
                quizData.SuggestedTags ??= [];
                quizData.Questions ??= [];

                // Keep only well-formed single-choice questions (4 choices, exactly one correct).
                var validQuestions = quizData.Questions
                    .Where(q => !string.IsNullOrWhiteSpace(q.Text)
                                && q.Choices?.Count == 4
                                && q.Choices.Count(c => c.IsCorrect) == 1)
                    .Select(q => q with { Type = QuestionType.SingleChoice })
                    .ToList();

                var skipped = quizData.Questions.Count - validQuestions.Count;
                if (skipped > 0)
                    _logger.LogWarning("Skipped {Count} malformed AI-generated question(s)", skipped);

                quizData.Questions = validQuestions;
            }

            return quizData;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON parsing error. Response: {Response}", jsonResponse);
            return null;
        }
    }
}
