using LanguageExt;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Auth.Shared.Infrastructure.Options;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Adapters;

public class ConfigurationAdminUsernameProvider(AdminOptions adminOptions) : IAdminUsernameProvider
{
    public Either<DomainError, AdminUsername> Get() => AdminUsername.Create(adminOptions.Username);
}
