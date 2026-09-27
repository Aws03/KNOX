using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Auth.Commands.SendVerificationOtp;

/// <summary>
/// Emails a one-time code (account verification / password reset). Runs in a transaction, so a
/// provider failure also rolls back the new code instead of throttling the user's retry.
/// </summary>
public sealed record SendVerificationOtpCommand(string Email) : IRequest<Result<Success>>;
