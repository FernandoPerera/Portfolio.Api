using Portfolio.Api.Auth.Shared.Domain.Models;

namespace Portfolio.Api.Tests.Auth.Shared.Domain.Builders;

public abstract class AdminUsernameBuilder
{
    public static AdminUsername Build(string username = "admin")
    {
        return AdminUsername.Create(username)
            .IfLeft(error => throw new Exception("Error in AdminUsernameBuilder, error: " + error.Message));
    }
}
