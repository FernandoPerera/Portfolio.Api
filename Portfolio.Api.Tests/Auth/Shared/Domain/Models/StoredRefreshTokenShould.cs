using Portfolio.Api.Auth.Shared.Domain.Errors;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Tests.Auth.Shared.Domain.Builders;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Shared.Domain.Models;

public class StoredRefreshTokenShould
{
    private static readonly DateTimeOffset Now = new(2026, 10, 16, 12, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset ExpiresAt = Now.AddDays(7);
    private static readonly Guid Id = Guid.NewGuid();

    [Fact]
    public void BeCreatedWithoutBeingConsumed()
    {
        StoredRefreshToken.Create(Id, "hash", "admin", ExpiresAt)
            .Match(
                created =>
                {
                    created.GetId().ShouldBe(Id);
                    created.GetTokenHash().ShouldBe("hash");
                    created.GetOwner().ShouldBe("admin");
                    created.GetExpiresAt().ShouldBe(ExpiresAt);
                    created.GetConsumedAt().ShouldBeNull();
                },
                error => error.ShouldBeNull()
            );
    }

    [Fact]
    public void BeConsumedWhenItIsValid()
    {
        StoredRefreshTokenBuilder.Build(ExpiresAt)
            .Consume(Now)
            .Match(
                consumed => consumed.GetConsumedAt().ShouldBe(Now),
                error => error.ShouldBeNull()
            );
    }

    [Fact]
    public void RejectConsumptionWhenItWasAlreadyConsumed()
    {
        StoredRefreshTokenBuilder.Build(ExpiresAt, consumedAt: Now.AddMinutes(-1))
            .Consume(Now)
            .Match(
                consumed => consumed.ShouldBeNull(),
                error => error.ShouldBeOfType<RefreshTokenReuseError>()
                    .Message.ShouldBe("Refresh token has already been used")
            );
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void RejectConsumptionWhenItHasExpired(int daysFromNow)
    {
        StoredRefreshTokenBuilder.Build(Now.AddDays(daysFromNow))
            .Consume(Now)
            .Match(
                consumed => consumed.ShouldBeNull(),
                error => error.ShouldBeOfType<UnauthorizedError>()
                    .Message.ShouldBe("Invalid refresh token")
            );
    }
}
