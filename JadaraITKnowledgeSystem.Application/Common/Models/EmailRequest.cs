using System;
using System.Collections.Generic;

namespace JadaraITKnowledgeSystem.Application.Common.Models;

public record EmailRequest
{
    public required EmailFrom from { get; init; }
    public required List<EmailRecipient> recipients { get; init; }
    public required string subject { get; init; }
    public string? text_content { get; init; }
    public string? html_content { get; init; }
}

public record EmailFrom
{
    public required string email { get; init; }
    public string? name { get; init; }
}

public record EmailRecipient
{
    public required string email { get; init; }
    public string? name { get; init; }
}
