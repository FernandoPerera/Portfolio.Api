using LanguageExt;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Adapters;

public class EntityFrameworkRefreshTokenConsumerAdapter(
    AppDbContext dbContext,
    TimeProvider timeProvider
) : IRefreshTokenConsumerRepository
{
    public async Task<Either<Error, AdminUsername>> Consume(RefreshToken refreshToken)
    {
        try
        {
            var now = timeProvider.GetUtcNow();
            var tokenHash = RefreshTokenHasher.Hash(refreshToken);
            var stored = await dbContext.RefreshTokens.FirstOrDefaultAsync(token => token.TokenHash == tokenHash);

            if (stored is null)
            {
                return InvalidRefreshToken();
            }

            if (stored.ConsumedAt is not null)
            {
                var session = dbContext.RefreshTokens.Where(token => token.Owner == stored.Owner);
                dbContext.RefreshTokens.RemoveRange(session);
                await dbContext.SaveChangesAsync();

                return InvalidRefreshToken();
            }

            if (stored.ExpiresAt <= now)
            {
                return InvalidRefreshToken();
            }

            stored.ConsumedAt = now;
            await dbContext.SaveChangesAsync();

            return AdminUsername.Create(stored.Owner);
        }
        catch (Exception error)
        {
            return Error.New(new ServiceUnavailableException("Database is unavailable, error: " + error.Message));
        }
    }

    private static Error InvalidRefreshToken() => Error.New(new UnauthorizedException("Invalid refresh token"));
}
