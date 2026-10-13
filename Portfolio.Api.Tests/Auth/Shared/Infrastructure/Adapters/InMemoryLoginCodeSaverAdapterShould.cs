using Microsoft.Extensions.Caching.Memory;
using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;
using Portfolio.Api.Tests.Auth.Shared.Domain.Builders;
using Portfolio.Api.Tests.Shared.Extensions;
using Portfolio.Api.Tests.Shared.Infrastructure;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Shared.Infrastructure.Adapters;

public class InMemoryLoginCodeSaverAdapterShould
{
    private static readonly DateTimeOffset Now = new(2026, 1, 1, 10, 0, 0, TimeSpan.Zero);

    private readonly MemoryCache _cache = new(new MemoryCacheOptions());
    private readonly InMemoryLoginCodeSaverAdapter _saver;

    public InMemoryLoginCodeSaverAdapterShould()
    {
        _saver = new InMemoryLoginCodeSaverAdapter(_cache, new FixedTimeProvider(Now));
    }

    [Fact]
    public void SaveTheHashOfTheLoginCodeWithItsExpiration()
    {
        var loginCode = LoginCodeBuilder.Build("login-code");

        _saver.Save(loginCode).Match(
            unit =>
            {
                unit.ShouldBeUnit();
                _cache.Get<DateTimeOffset>(LoginCodeHasher.Hash(loginCode))
                    .ShouldBe(Now.Add(InMemoryLoginCodeSaverAdapter.Lifetime));
            },
            error => error.ShouldBeNull()
        );
    }
}
