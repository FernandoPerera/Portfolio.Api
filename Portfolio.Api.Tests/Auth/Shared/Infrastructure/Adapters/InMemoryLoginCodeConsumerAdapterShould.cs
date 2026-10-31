using Microsoft.Extensions.Caching.Memory;
using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Tests.Auth.Shared.Domain.Builders;
using Portfolio.Api.Tests.Shared.Extensions;
using Portfolio.Api.Tests.Shared.Infrastructure;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Shared.Infrastructure.Adapters;

public class InMemoryLoginCodeConsumerAdapterShould
{
    private static readonly DateTimeOffset Now = new(2026, 1, 1, 10, 0, 0, TimeSpan.Zero);

    private readonly MemoryCache _cache = new(new MemoryCacheOptions());
    private readonly InMemoryLoginCodeConsumerAdapter _consumer;

    public InMemoryLoginCodeConsumerAdapterShould()
    {
        _consumer = new InMemoryLoginCodeConsumerAdapter(_cache, new FixedTimeProvider(Now));
    }

    [Fact]
    public void ConsumeAValidLoginCode()
    {
        var loginCode = LoginCodeBuilder.Build("login-code");
        _cache.Set(LoginCodeHasher.Hash(loginCode), Now.AddSeconds(30));

        _consumer.Consume(loginCode).Match(
            unit =>
            {
                unit.ShouldBeUnit();
                _cache.TryGetValue(LoginCodeHasher.Hash(loginCode), out _).ShouldBeFalse();
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public void RejectAnUnknownLoginCode()
    {
        _consumer.Consume(LoginCodeBuilder.Build("unknown")).Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error => error.ShouldBeOfType<UnauthorizedError>()
                .Message.ShouldBe("Invalid login code")
        );
    }

    [Fact]
    public void RejectAnExpiredLoginCode()
    {
        var loginCode = LoginCodeBuilder.Build("expired");
        _cache.Set(LoginCodeHasher.Hash(loginCode), Now.AddSeconds(-1));

        _consumer.Consume(loginCode).Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error => error.ShouldBeOfType<UnauthorizedError>()
        );
    }

    [Fact]
    public void RejectALoginCodeThatWasAlreadyUsed()
    {
        var loginCode = LoginCodeBuilder.Build("used");
        _cache.Set(LoginCodeHasher.Hash(loginCode), Now.AddSeconds(30));
        _consumer.Consume(loginCode);

        _consumer.Consume(loginCode).Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error => error.ShouldBeOfType<UnauthorizedError>()
        );
    }
}
