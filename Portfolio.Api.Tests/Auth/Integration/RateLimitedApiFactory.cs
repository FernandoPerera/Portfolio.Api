using Microsoft.AspNetCore.Hosting;

namespace Portfolio.Api.Tests.Auth.Integration;

public class RateLimitedApiFactory : PortfolioApiFactory
{
    public const int PermitLimit = 2;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.UseSetting("RateLimiting:Auth:PermitLimit", PermitLimit.ToString());
        builder.UseSetting("RateLimiting:Public:PermitLimit", PermitLimit.ToString());
    }
}
