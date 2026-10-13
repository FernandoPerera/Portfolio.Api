using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;
using Portfolio.Api.Auth.Shared.Infrastructure.Options;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Tests.Auth.Shared.Domain.Builders;
using Portfolio.Api.Tests.Shared.Extensions;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Shared.Infrastructure.Adapters;

public class ConfigurationAdminAuthorizerShould
{
    private const string AdminUserId = "123456";

    [Fact]
    public void AcceptTheConfiguredAdmin()
    {
        var authorizer = CreateAuthorizer(AdminUserId);

        authorizer.EnsureIsAdmin(ExternalUserIdBuilder.Build(AdminUserId))
            .Match(
                unit => unit.ShouldBeUnit(),
                error => error.ShouldBeNull()
            );
    }

    [Theory]
    [InlineData(AdminUserId, "654321")]
    [InlineData("", AdminUserId)]
    [InlineData("   ", AdminUserId)]
    public void RejectAnyUserWhenTheyAreNotTheConfiguredAdmin(string configuredAdminId, string givenUserId)
    {
        var authorizer = CreateAuthorizer(configuredAdminId);

        authorizer.EnsureIsAdmin(ExternalUserIdBuilder.Build(givenUserId))
            .Match(
                unit => unit.ShouldBeEquivalentTo(null),
                error => error.Exception.Case.ShouldBeOfType<ForbiddenException>()
                    .Message.ShouldBe("This user is not allowed to manage the portfolio")
            );
    }

    private static ConfigurationAdminAuthorizer CreateAuthorizer(string configuredAdminId) =>
        new(new AdminOptions { GitHubUserId = configuredAdminId });
}
