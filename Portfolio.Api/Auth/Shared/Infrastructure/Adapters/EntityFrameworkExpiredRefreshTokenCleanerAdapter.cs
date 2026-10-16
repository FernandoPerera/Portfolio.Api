using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Shared.Infrastructure.Data;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Adapters;

public class EntityFrameworkExpiredRefreshTokenCleanerAdapter(
    AppDbContext dbContext,
    TimeProvider timeProvider
) : IExpiredRefreshTokenCleanerRepository
{
    public async Task<Either<Error, Unit>> Clean()
    {
        try
        {
            var now = timeProvider.GetUtcNow();
            var expired = dbContext.RefreshTokens.Where(token => token.ExpiresAt <= now);
            dbContext.RefreshTokens.RemoveRange(expired);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            // Cleaning is best effort: a failure here must never break a login or a refresh.
            dbContext.ChangeTracker.Clear();
        }

        return Unit.Default;
    }
}
