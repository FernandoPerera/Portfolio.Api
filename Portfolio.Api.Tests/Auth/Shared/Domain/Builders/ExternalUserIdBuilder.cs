using Portfolio.Api.Auth.Shared.Domain.Models;

namespace Portfolio.Api.Tests.Auth.Shared.Domain.Builders;

public abstract class ExternalUserIdBuilder
{
    public static ExternalUserId Build(string id = "123456")
    {
        return ExternalUserId.Create(id)
            .IfLeft(error => throw new Exception("Error in ExternalUserIdBuilder, error: " + error.Message));
    }
}
