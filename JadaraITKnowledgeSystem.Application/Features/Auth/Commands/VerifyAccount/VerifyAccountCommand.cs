using JadaraITKnowledgeSystem.Application.Features.Auth.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Auth.Commands.VerifyAccount;

/// <summary>Confirms the account's email with an OTP and signs the user in.</summary>
public sealed record VerifyAccountCommand(string Email, string Otp, string IpAddress) : IRequest<Result<AuthTokensDto>>;
