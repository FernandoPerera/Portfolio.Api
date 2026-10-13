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
    JwtOptions jwtOptions,
    TimeProvider timeProvider
) : IRefreshTokenSaverRepository
{
    public async Task<Either<Error, Unit>> Save(RefreshToken refreshToken, AdminUsername owner)
    {
        try
        {
            var entity = new RefreshTokenEntity
            {
                Id = generatorIdentifier.Generate(),
                TokenHash = RefreshTokenHasher.Hash(refreshToken),
                Owner = owner.ToString(),
                ExpiresAt = timeProvider.GetUtcNow().AddDays(jwtOptions.RefreshTokenDays)
            };

            await dbContext.RefreshTokens.AddAsync(entity);
            await dbContext.SaveChangesAsync();

            return Unit.Default;
        }
        catch (Exception error)
        {
            return Error.New(new ServiceUnavailableException("Database is unavailable, error: " + error.Message));
        }
    }
}
