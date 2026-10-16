using Portfolio.Api.Auth.Shared.Domain.Models;

namespace Portfolio.Api.Tests.Auth.Shared.Domain.Builders;

public abstract class ExpiresAtBuilder
{
    public static ExpiresAt Build(DateTimeOffset expiresAt)
    {
        return ExpiresAt.Create(expiresAt)
            .IfLeft(error => throw new Exception("Error in ExpiresAtBuilder, error: " + error.Message));
    }
}
