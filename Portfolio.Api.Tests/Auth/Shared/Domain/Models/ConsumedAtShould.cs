using Portfolio.Api.Auth.Shared.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Shared.Domain.Models;

public class ConsumedAtShould
{
    private static readonly DateTimeOffset Moment = new(2026, 10, 16, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void BeCreated()
    {
        ConsumedAt.Create(Moment)
            .Match(
                created =>
                {
                    created.IsConsumed().ShouldBeTrue();
                    created.GetValue().ShouldBe(Moment);
                },
                error => error.ShouldBeNull()
            );
    }

    [Fact]
    public void BeCreatedWithoutBeingConsumed()
    {
        ConsumedAt.NotConsumed()
            .Match(
                created =>
                {
                    created.IsConsumed().ShouldBeFalse();
                    created.GetValue().ShouldBeNull();
                },
                error => error.ShouldBeNull()
            );
    }
}
