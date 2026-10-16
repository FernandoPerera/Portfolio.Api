using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Tests.Auth.Shared.Domain.Builders;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Shared.Domain.Models;

public class ExpiresAtShould
{
    private static readonly DateTimeOffset Moment = new(2026, 10, 16, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void BeCreated()
    {
        ExpiresAt.Create(Moment)
            .Match(
                created => created.GetValue().ShouldBe(Moment),
                error => error.ShouldBeNull()
            );
    }

    [Theory]
    [InlineData(-1, false)]
    [InlineData(0, true)]
    [InlineData(1, true)]
    public void KnowWhenItHasExpired(int daysAfterExpiration, bool expected)
    {
        ExpiresAtBuilder.Build(Moment)
            .HasExpired(ConsumedAtBuilder.Build(Moment.AddDays(daysAfterExpiration)))
            .ShouldBe(expected);
    }
}
