using LanguageExt;
using LanguageExt.Common;
using Microsoft.Extensions.Caching.Memory;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Adapters;

public class InMemoryLoginCodeSaverAdapter(
    IMemoryCache cache,
    TimeProvider timeProvider
) : ILoginCodeSaverRepository
{
    public static readonly TimeSpan Lifetime = TimeSpan.FromSeconds(60);

    public Either<Error, Unit> Save(LoginCode loginCode)
    {
        cache.Set(LoginCodeHasher.Hash(loginCode), timeProvider.GetUtcNow().Add(Lifetime), Lifetime);

        return Unit.Default;
    }
}
