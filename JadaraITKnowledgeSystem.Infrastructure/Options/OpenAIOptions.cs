namespace JadaraITKnowledgeSystem.Infrastructure.Options;

public sealed class OpenAIOptions
{
    public const string SectionName = "OpenAI";

    /// <summary>Optional: without a key, quiz generation jobs fail with a clear error instead of the app failing to start.</summary>
    public string? ApiKey { get; set; }
    public string Model { get; set; } = "gpt-4.1-mini";
    public int MaxTokens { get; set; } = 4000;
    public double Temperature { get; set; } = 0.7;
    public string Endpoint { get; set; } = "https://api.openai.com/v1/chat/completions";

    public bool IsConfigured => !string.IsNullOrWhiteSpace(ApiKey);
}
