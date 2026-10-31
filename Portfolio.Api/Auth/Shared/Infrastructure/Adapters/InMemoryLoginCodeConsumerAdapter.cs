using LanguageExt;
using Microsoft.Extensions.Caching.Memory;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Adapters;

public class InMemoryLoginCodeConsumerAdapter(
    IMemoryCache cache,
    TimeProvider timeProvider
) : ILoginCodeConsumerRepository
{
    private readonly Lock _consumeLock = new();

    public Either<DomainError, Unit> Consume(LoginCode loginCode)
    {
        lock (_consumeLock)
        {
            var key = LoginCodeHasher.Hash(loginCode);
            var found = cache.TryGetValue(key, out DateTimeOffset expiresAt);
            cache.Remove(key);

            if (!found || expiresAt <= timeProvider.GetUtcNow())
            {
                return new UnauthorizedError("Invalid login code");
            }

            return Unit.Default;
        }
    }
}
