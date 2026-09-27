using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Infrastructure.Options;
using JadaraITKnowledgeSystem.Infrastructure.Services.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JadaraITKnowledgeSystem.Infrastructure.Services.JWT
{
    public sealed class JwtTokenService(IOptions<JwtOptions> options, TimeProvider timeProvider) : IJwtTokenService
    {
        public Task<string> GenerateJwtTokenAsync(int userId, int domainUserId, string? fullName, string? email, IEnumerable<string> roles)
        {
            var settings = options.Value;
            var now = timeProvider.GetUtcNow();

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userId.ToString(CultureInfo.InvariantCulture)),
                new(CustomClaimTypes.DomainUserId, domainUserId.ToString(CultureInfo.InvariantCulture)),
                new(ClaimTypes.Name, fullName ?? string.Empty),
                new(ClaimTypes.Email, email ?? string.Empty),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Only one role claim: the caller passes the user's highest role.
            var singleRole = roles.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(singleRole))
                claims.Add(new Claim(ClaimTypes.Role, singleRole));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                IssuedAt = now.UtcDateTime,
                NotBefore = now.UtcDateTime,
                Expires = now.UtcDateTime.AddMinutes(settings.ExpirationMinutes),
                Issuer = settings.Issuer,
                Audience = settings.Audience,
                SigningCredentials = new SigningCredentials(SigningKey(settings), SecurityAlgorithms.HmacSha256)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            return Task.FromResult(tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor)));
        }

        public static SymmetricSecurityKey SigningKey(JwtOptions settings) => new(Encoding.UTF8.GetBytes(settings.Secret));
    }
}
