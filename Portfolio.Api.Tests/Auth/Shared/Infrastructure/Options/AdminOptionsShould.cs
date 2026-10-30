using Portfolio.Api.Auth.Shared.Infrastructure.Options;
using Portfolio.Api.Tests.Shared.Extensions;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Shared.Infrastructure.Options;

public class AdminOptionsShould
{
    private const string ValidUsername = "admin";
    private const string ValidGitHubUserId = "123456";

    [Fact]
    public void BeValid()
    {
        CreateOptions().GetInvalidMembers().ShouldBeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void RejectTheUsernameWhenItIsEmpty(string username)
    {
        CreateOptions(username: username).GetInvalidMembers().ShouldBe([nameof(AdminOptions.Username)]);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not-a-number")]
    [InlineData("123 456")]
    [InlineData("-123456")]
    public void RejectTheGitHubUserIdWhenItIsNotNumeric(string gitHubUserId)
    {
        CreateOptions(gitHubUserId: gitHubUserId).GetInvalidMembers().ShouldBe([nameof(AdminOptions.GitHubUserId)]);
    }

    private static AdminOptions CreateOptions(string username = ValidUsername, string gitHubUserId = ValidGitHubUserId) =>
        new() { Username = username, GitHubUserId = gitHubUserId };
}
