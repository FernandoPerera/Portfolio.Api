namespace Portfolio.Api.Tests.Shared.Infrastructure;

public class FixedTimeProvider(DateTimeOffset now) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => now;
}
