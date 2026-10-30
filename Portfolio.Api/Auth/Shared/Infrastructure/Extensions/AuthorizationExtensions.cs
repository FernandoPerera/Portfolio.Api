using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Portfolio.Api.Auth.Shared.Infrastructure.Authorization;
using Portfolio.Api.Auth.Shared.Infrastructure.Options;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Extensions;

public static class AuthorizationExtensions
{
    public static void AddAdminAuthorization(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AdminOptions>()
            .Bind(configuration.GetSection(AdminOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddSingleton(provider => provider.GetRequiredService<IOptions<AdminOptions>>().Value);

        var adminPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireClaim(AdminAccess.RoleClaimType, AdminAccess.RoleClaimValue)
            .Build();

        services.AddAuthorizationBuilder()
            .AddPolicy(AdminAccess.PolicyName, adminPolicy)
            .SetDefaultPolicy(adminPolicy)
            .SetFallbackPolicy(adminPolicy);
    }
}
