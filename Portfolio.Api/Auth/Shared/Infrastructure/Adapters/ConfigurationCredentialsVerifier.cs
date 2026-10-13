using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Auth.Shared.Infrastructure.Options;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Adapters;

public class ConfigurationCredentialsVerifier(AdminOptions admin) : ICredentialsVerifier
{
    public Either<Error, Unit> Verify(AdminCredentials credentials)
    {
        var isUsernameValid = credentials.GetUsername().ToString() == admin.Username;
        var isPasswordValid = Pbkdf2PasswordHasher.Verify(credentials.GetPassword(), admin.PasswordHash);

        if (!isUsernameValid || !isPasswordValid)
        {
            return Error.New(new UnauthorizedException("Invalid credentials"));
        }

        return Unit.Default;
    }
}
