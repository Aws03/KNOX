using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace JadaraITKnowledgeSystem.Infrastructure.Services.Security
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IApplicationDbContext _context;

        public RefreshTokenService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<RefreshToken>> GenerateRefreshTokenAsync(int userId, string ipAddress)
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var expiresAt = DateTime.UtcNow.AddDays(7);

            var refreshToken = RefreshToken.Create(userId, token, expiresAt, ipAddress);

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync(CancellationToken.None);

            return refreshToken;
        }

        public async Task<Result<RefreshToken>> GetActiveRefreshTokenAsync(string token)
        {
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token);

            if (refreshToken is null)
                return Error.NotFound(description: "Refresh token not found");

            if (!refreshToken.IsActive)
                return Error.Validation(description: "Refresh token is not active");

            return refreshToken;
        }

        public async Task<Result<Success>> RevokeTokenAsync(string token, string ipAddress)
        {
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token);

            if (refreshToken is null)
                return Error.NotFound(description: "Refresh token not found");

            refreshToken.Revoke(ipAddress);
            await _context.SaveChangesAsync(CancellationToken.None);

            return Result.Success;
        }

        public async Task<Result<Success>> RevokeAllUserTokensAsync(int userId, string ipAddress)
        {
            var tokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.IsRevoked == false)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.Revoke(ipAddress);
            }

            await _context.SaveChangesAsync(CancellationToken.None);
            return Result.Success;
        }
    }
}
