using LanguageExt;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Auth.Shared.Domain.Errors;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Adapters;

public class EntityFrameworkRefreshTokenConsumerAdapter(AppDbContext dbContext) : IRefreshTokenConsumerRepository
{
    public async Task<Either<Error, Unit>> Consume(StoredRefreshToken consumedRefreshToken)
    {
        try
        {
            var entity = await dbContext.RefreshTokens.FindAsync(consumedRefreshToken.GetId());

            if (entity is null)
            {
                return Error.New(new UnauthorizedException("Invalid refresh token"));
            }

            var entry = dbContext.Entry(entity);
            entry.Property(token => token.ConsumedAt).OriginalValue = null;
            entry.Property(token => token.ConsumedAt).CurrentValue = consumedRefreshToken.GetConsumedAt();
            await dbContext.SaveChangesAsync();

            return Unit.Default;
        }
        catch (DbUpdateConcurrencyException)
        {
            return await ReuseOrUnknown(consumedRefreshToken);
        }
        catch (Exception error)
        {
            return Error.New(new ServiceUnavailableException("Database is unavailable, error: " + error.Message, error));
        }
    }

    private async Task<Either<Error, Unit>> ReuseOrUnknown(StoredRefreshToken consumedRefreshToken)
    {
        try
        {
            var id = consumedRefreshToken.GetId();
            var stillExists = await dbContext.RefreshTokens.AsNoTracking().AnyAsync(token => token.Id == id);

            return stillExists
                ? Error.New(new RefreshTokenReuseException("Refresh token has already been used"))
                : Error.New(new UnauthorizedException("Invalid refresh token"));
        }
        catch (Exception error)
        {
            return Error.New(new ServiceUnavailableException("Database is unavailable, error: " + error.Message, error));
        }
    }
}
