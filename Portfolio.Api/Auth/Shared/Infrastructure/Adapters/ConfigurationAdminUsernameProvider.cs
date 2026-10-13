using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Auth.Shared.Infrastructure.Options;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Adapters;

public class ConfigurationAdminUsernameProvider(AdminOptions adminOptions) : IAdminUsernameProvider
{
    public Either<Error, AdminUsername> Get() => AdminUsername.Create(adminOptions.Username);
}
