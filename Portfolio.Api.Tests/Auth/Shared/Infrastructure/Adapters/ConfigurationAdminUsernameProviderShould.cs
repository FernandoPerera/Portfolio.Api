using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;
using Portfolio.Api.Auth.Shared.Infrastructure.Options;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Shared.Infrastructure.Adapters;

public class ConfigurationAdminUsernameProviderShould
{
    [Fact]
    public void ProvideTheConfiguredUsername()
    {
        new ConfigurationAdminUsernameProvider(new AdminOptions { Username = "admin" }).Get()
            .Match(
                username => username.ToString().ShouldBe("admin"),
                error => error.ShouldBeNull()
            );
    }
}
