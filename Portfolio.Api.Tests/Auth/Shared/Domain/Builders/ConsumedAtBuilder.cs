using Portfolio.Api.Auth.Shared.Domain.Models;

namespace Portfolio.Api.Tests.Auth.Shared.Domain.Builders;

public abstract class ConsumedAtBuilder
{
    public static ConsumedAt Build(DateTimeOffset consumedAt)
    {
        return ConsumedAt.Create(consumedAt)
            .IfLeft(error => throw new Exception("Error in ConsumedAtBuilder, error: " + error.Message));
    }
}
