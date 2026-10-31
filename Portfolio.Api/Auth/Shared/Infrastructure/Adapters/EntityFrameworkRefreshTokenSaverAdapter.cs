using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Auth.Shared.Infrastructure.Entities;
using Portfolio.Api.Auth.Shared.Infrastructure.Options;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Domain.Ports;
using Portfolio.Api.Shared.Infrastructure.Data;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Adapters;

public class EntityFrameworkRefreshTokenSaverAdapter(
    AppDbContext dbContext,
    IGeneratorIdentifier generatorIdentifier,
    IRefreshTokenHasher refreshTokenHasher,
    JwtOptions jwtOptions,
    TimeProvider timeProvider
) : IRefreshTokenSaverRepository
{
    public async Task<Either<Error, Unit>> Save(RefreshToken refreshToken, AdminUsername owner)
    {
        return await (
            from tokenHash in refreshTokenHasher.Hash(refreshToken).ToAsync()
            from stored in StoredRefreshToken.Create(
                generatorIdentifier.Generate(),
                tokenHash.ToString(),
                owner.ToString(),
                timeProvider.GetUtcNow().AddDays(jwtOptions.RefreshTokenDays)
            ).ToAsync()
            from _ in Persist(stored).ToAsync()
            select Unit.Default
        );
    }

    private async Task<Either<Error, Unit>> Persist(StoredRefreshToken stored)
    {
        try
        {
            var entity = new RefreshTokenEntity
            {
                Id = stored.GetId(),
                TokenHash = stored.GetTokenHash(),
                Owner = stored.GetOwner(),
                ExpiresAt = stored.GetExpiresAt(),
                ConsumedAt = stored.GetConsumedAt()
            };

            await dbContext.RefreshTokens.AddAsync(entity);
            await dbContext.SaveChangesAsync();

            return Unit.Default;
        }
        catch (Exception error)
        {
            return Error.New(new ServiceUnavailableException("Database is unavailable, error: " + error.Message, error));
        }
    }
}
