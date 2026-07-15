using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Quizzes.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Infrastructure.Services.AI;

/// <summary>
/// Service for interacting with OpenAI API to generate quizzes from text.
/// Requires: dotnet add package OpenAI or Azure.AI.OpenAI
/// </summary>
public class OpenAIService : IOpenAIService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenAIService> _logger;
    private readonly string _apiKey;
    private readonly string _model;
    private readonly int _maxTokens;
    private readonly double _temperature;

    public OpenAIService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<OpenAIService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        var openAIConfig = configuration.GetSection("OpenAI");
        _apiKey = openAIConfig["ApiKey"] ?? throw new InvalidOperationException("OpenAI:ApiKey not configured");
        _model = openAIConfig["Model"] ?? "gpt-4-turbo-preview";
        _maxTokens = int.Parse(openAIConfig["MaxTokens"] ?? "4000");
        _temperature = double.Parse(openAIConfig["Temperature"] ?? "0.7");

        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }

    public async Task<Result<GeneratedQuizDto>> GenerateQuizFromTextAsync(
        GenerateQuizRequest request,
        CancellationToken cancellationToken = default)
    {
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

    public async Task<Result<string>> GenerateTextAsync(
        string prompt,
        CancellationToken cancellationToken = default)
    {
        return await CallOpenAIAsync(prompt, cancellationToken);
    }

    public async Task<Result<List<string>>> ExtractTopicsAsync(
        string text,
        int maxTopics = 5,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var prompt = $@"
Extract {maxTopics} key topics/keywords from the following educational content.
Return only a JSON array of strings with the topics, no other text.
Each topic should be 1-3 words maximum.

Content:
{text.Substring(0, Math.Min(2000, text.Length))}

Return format: [""topic1"", ""topic2"", ""topic3""]
";

            var response = await CallOpenAIAsync(prompt, cancellationToken);
            if (response.IsError)
                return response.Errors;

            var topics = JsonSerializer.Deserialize<List<string>>(response.Value);
            return topics ?? new List<string>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting topics");
            return new List<string>(); // Return empty list on error
        }
    }

    private async Task<Result<string>> CallOpenAIAsync(string prompt, CancellationToken cancellationToken)
    {
        try
        {
            var requestBody = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "system", content = GetSystemPrompt() },
                    new { role = "user", content = prompt }
                },
                max_tokens = _maxTokens,
                temperature = _temperature
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(
                "https://api.openai.com/v1/chat/completions",
                content,
                cancellationToken);

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

    private string GetSystemPrompt()
    {
        return @"You are an expert educational quiz generator. Your task is to create high-quality, 
educational multiple-choice quizzes based on provided content. Always return responses in valid JSON format.
Be precise, educational, and ensure questions are clear and unambiguous.";
    }

    private string BuildQuizGenerationPrompt(GenerateQuizRequest request)
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
      ""type"": 0,
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
                quizData.Topic ??= "General Quiz";
                quizData.Title ??= "Generated Quiz";
                quizData.Description ??= "Test your knowledge with this quiz.";
                quizData.SuggestedTags ??= new List<string>();
                quizData.Questions ??= new List<CreateQuestionDto>();

                // Ensure all questions have type set
                foreach (var question in quizData.Questions)
                {
                    if (question.Choices?.Count == 4 && question.Choices.Count(c => c.IsCorrect) == 1)
                    {
                        // Valid question
                        continue;
                    }
                    else
                    {
                        _logger.LogWarning("Invalid question detected, skipping");
                    }
                }

                // Filter out invalid questions
                quizData.Questions = quizData.Questions
                    .Where(q => q.Choices?.Count == 4 && q.Choices.Count(c => c.IsCorrect) == 1)
                    .ToList();
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
