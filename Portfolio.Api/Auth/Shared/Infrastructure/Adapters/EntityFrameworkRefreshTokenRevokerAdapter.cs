using LanguageExt;
using Microsoft.EntityFrameworkCore;
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
            await dbContext.RefreshTokens.Where(token => token.Owner == owner).ExecuteDeleteAsync();

            return Unit.Default;
        }
        catch (Exception error)
        {
            return new ServiceUnavailableError("Database is unavailable, error: " + error.Message, error);
        }
    }
}
