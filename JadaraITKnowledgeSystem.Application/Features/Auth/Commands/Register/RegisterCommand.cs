using JadaraITKnowledgeSystem.Application.Features.Auth.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Users.Commands.CreateUser;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Auth.Commands.Register;

public sealed record RegisterCommand(
    string FullName,
    string Email,
    int MajorId,
    string? Password,
    string IpAddress
) : IRequest<Result<RegistrationResultDto>>;

/// <summary>
/// Either <see cref="RequiresVerification"/> is true and an OTP was (attempted to be)
/// emailed, or it is false and <see cref="Tokens"/> signs the new user straight in.
/// </summary>
public sealed record RegistrationResultDto(
    CreateUserResultDto User,
    bool RequiresVerification,
    bool OtpSent,
    AuthTokensDto? Tokens);
