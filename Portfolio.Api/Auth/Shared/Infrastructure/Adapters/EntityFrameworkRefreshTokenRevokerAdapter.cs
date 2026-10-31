using LanguageExt;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Adapters;

public class EntityFrameworkRefreshTokenRevokerAdapter(AppDbContext dbContext) : IRefreshTokenRevokerRepository
{
    public async Task<Either<DomainError, Unit>> Revoke(StoredRefreshToken refreshToken)
    {
        try
        {
            var owner = refreshToken.GetOwner();
            var session = dbContext.RefreshTokens.Where(token => token.Owner == owner);
            dbContext.RefreshTokens.RemoveRange(session);
            await dbContext.SaveChangesAsync();

            return Unit.Default;
        }
        catch (Exception error)
        {
            return new ServiceUnavailableError("Database is unavailable, error: " + error.Message, error);
        }
    }
}
