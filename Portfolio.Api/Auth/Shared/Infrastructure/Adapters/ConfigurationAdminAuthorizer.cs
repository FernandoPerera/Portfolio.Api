using LanguageExt;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Auth.Shared.Infrastructure.Options;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Adapters;

public class ConfigurationAdminAuthorizer(AdminOptions adminOptions) : IAdminAuthorizer
{
    public Either<DomainError, Unit> EnsureIsAdmin(ExternalUserId userId)
    {
        var adminUserId = adminOptions.GitHubUserId;
        if (string.IsNullOrWhiteSpace(adminUserId) || adminUserId.Trim() != userId.ToString())
        {
            return new ForbiddenError($"GitHub user '{userId}' is not allowed to manage the portfolio");
        }

        return Unit.Default;
    }
}
