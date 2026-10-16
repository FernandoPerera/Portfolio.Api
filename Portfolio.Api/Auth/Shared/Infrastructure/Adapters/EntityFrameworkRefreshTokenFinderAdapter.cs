using LanguageExt;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Adapters;

public class EntityFrameworkRefreshTokenFinderAdapter(AppDbContext dbContext) : IRefreshTokenFinderRepository
{
    public async Task<Either<Error, StoredRefreshToken>> Find(RefreshTokenHash tokenHash)
    {
        try
        {
            var entity = await dbContext.RefreshTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(token => token.TokenHash == tokenHash.ToString());

            if (entity is null)
            {
                return Error.New(new UnauthorizedException("Invalid refresh token"));
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
            return Error.New(new ServiceUnavailableException("Database is unavailable, error: " + error.Message));
        }
    }
}
