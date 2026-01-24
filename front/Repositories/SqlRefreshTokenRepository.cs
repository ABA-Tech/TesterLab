using Auth.Core.Abstractions;
using Auth.Core.Models;
using Microsoft.EntityFrameworkCore;
using TesterLab.Data;

namespace TesterLab.Repositories
{
  public class SqlRefreshTokenRepository : IRefreshTokenRepository
  {
    private readonly ApplicationDbContext _context;

    public SqlRefreshTokenRepository(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
      return await _context.RefreshTokens
          .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task AddAsync(RefreshToken refreshToken)
    {
      _context.RefreshTokens.Add(refreshToken);
      await _context.SaveChangesAsync();
    }

    public async Task RevokeAsync(string token, string reason)
    {
      var refreshToken = await GetByTokenAsync(token);
      if (refreshToken != null)
      {
        refreshToken.IsRevoked = true;
        refreshToken.RevokedAt = DateTime.UtcNow;
        refreshToken.RevokedReason = reason;
        await _context.SaveChangesAsync();
      }
    }

    public async Task RevokeAllForUserAsync(string userId, string reason)
    {
      var tokens = await _context.RefreshTokens
          .Where(rt => rt.UserId == userId && !rt.IsRevoked)
          .ToListAsync();

      foreach (var token in tokens)
      {
        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;
        token.RevokedReason = reason;
      }

      await _context.SaveChangesAsync();
    }

    public async Task<bool> IsActiveAsync(string token)
    {
      var refreshToken = await GetByTokenAsync(token);
      return refreshToken?.IsActive ?? false;
    }

    public async Task CleanupExpiredTokensAsync()
    {
      var expiredTokens = await _context.RefreshTokens
          .Where(rt => rt.ExpiresAt < DateTime.UtcNow)
          .ToListAsync();

      _context.RefreshTokens.RemoveRange(expiredTokens);
      await _context.SaveChangesAsync();
    }
  }
}
