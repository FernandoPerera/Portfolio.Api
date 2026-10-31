using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Adapters;

public class EntityFrameworkExpiredRefreshTokenCleanerAdapter(
    AppDbContext dbContext,
    TimeProvider timeProvider,
    ILogger<EntityFrameworkExpiredRefreshTokenCleanerAdapter> logger
) : IExpiredRefreshTokenCleanerRepository
{
    public async Task<Either<DomainError, Unit>> Clean()
    {
        try
        {
            var now = timeProvider.GetUtcNow();
            await dbContext.RefreshTokens.Where(token => token.ExpiresAt <= now).ExecuteDeleteAsync();
        }
        catch (Exception error)
        {
            // Cleaning is best effort: a failure here must never break a login or a refresh.
            logger.LogWarning(error, "The expired refresh tokens could not be removed");
        }

        return Unit.Default;
    }
}
