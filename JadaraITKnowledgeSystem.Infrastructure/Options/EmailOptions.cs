namespace JadaraITKnowledgeSystem.Infrastructure.Options;

/// <summary>Brevo is used when its API key is set, otherwise AhaSend, otherwise emails are only logged.</summary>
public sealed class BrevoOptions
{
    public const string SectionName = "Brevo";

    public string? ApiKey { get; set; }
    public string FromEmail { get; set; } = "aws.03.dev@gmail.com";
    public string FromName { get; set; } = "KNOX";

    public bool IsConfigured => !string.IsNullOrWhiteSpace(ApiKey);
}

public sealed class AhaSendOptions
{
    public const string SectionName = "AhaSend";

    public string? AccountId { get; set; }
    public string? ApiKey { get; set; }
    public string? FromEmail { get; set; }
    public string FromName { get; set; } = "KNOX";

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(AccountId) && !string.IsNullOrWhiteSpace(ApiKey) && !string.IsNullOrWhiteSpace(FromEmail);
}
