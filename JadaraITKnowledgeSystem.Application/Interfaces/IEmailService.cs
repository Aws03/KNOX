using JadaraITKnowledgeSystem.Application.Common.Models;

namespace JadaraITKnowledgeSystem.Application.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailAsync(EmailRequest emailRequest);
    EmailFrom GetDefaultFrom();
}
