using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Adapters;

public class EntityFrameworkRefreshTokenFinderAdapter(AppDbContext dbContext) : IRefreshTokenFinderRepository
{
    public async Task<Either<DomainError, StoredRefreshToken>> Find(RefreshTokenHash tokenHash)
    {
        try
        {
            var entity = await dbContext.RefreshTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(token => token.TokenHash == tokenHash.ToString());

            if (entity is null)
            {
                return new UnauthorizedError("Invalid refresh token");
            }

            return StoredRefreshToken.CreateUnsafe(
                entity.Id,
                entity.TokenHash,
                entity.Owner,
                entity.ExpiresAt,
                entity.ConsumedAt
            );
        }
        catch (Exception error)
        {
            return new ServiceUnavailableError("Database is unavailable, error: " + error.Message, error);
        }
    }
}
