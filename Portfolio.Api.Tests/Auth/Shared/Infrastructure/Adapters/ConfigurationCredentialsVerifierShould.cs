using Microsoft.Extensions.Options;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Infrastructure.Adapters;
using Portfolio.Api.Auth.Shared.Infrastructure.Options;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Tests.Shared.Extensions;
using Shouldly;

namespace Portfolio.Api.Tests.Auth.Shared.Infrastructure.Adapters;

public class ConfigurationCredentialsVerifierShould
{
    private readonly ConfigurationCredentialsVerifier _verifier;

    public ConfigurationCredentialsVerifierShould()
    {
        var admin = new AdminOptions { Username = "admin", PasswordHash = Pbkdf2PasswordHasher.Hash("secret") };
        _verifier = new ConfigurationCredentialsVerifier(Options.Create(admin));
    }

    [Fact]
    public void AcceptTheAdminCredentials()
    {
        _verifier.Verify(CredentialsFor("admin", "secret"))
            .Match(
                unit => unit.ShouldBeUnit(),
                error => error.ShouldBeNull()
            );
    }

    [Theory]
    [InlineData("admin", "wrong")]
    [InlineData("other", "secret")]
    public void RejectCredentialsWhenTheyDoNotMatchTheAdmin(string username, string password)
    {
        _verifier.Verify(CredentialsFor(username, password))
            .Match(
                unit => unit.ShouldBeEquivalentTo(null),
                error => error.Exception.Case.ShouldBeOfType<UnauthorizedException>()
                    .Message.ShouldBe("Invalid credentials")
            );
    }

    private static AdminCredentials CredentialsFor(string username, string password) =>
        AdminCredentials.Create(username, password)
            .IfLeft(error => throw new Exception("Error building credentials, error: " + error.Message));
}
