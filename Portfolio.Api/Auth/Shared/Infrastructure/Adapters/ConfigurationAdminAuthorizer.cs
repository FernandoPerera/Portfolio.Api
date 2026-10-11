using LanguageExt;
using LanguageExt.Common;
using Microsoft.Extensions.Options;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Auth.Shared.Infrastructure.Options;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Adapters;

public class ConfigurationAdminAuthorizer(IOptions<AdminOptions> adminOptions) : IAdminAuthorizer
{
    public Either<Error, Unit> EnsureIsAdmin(ExternalUserId userId)
    {
        var adminUserId = adminOptions.Value.GitHubUserId;
        if (string.IsNullOrWhiteSpace(adminUserId) || adminUserId.Trim() != userId.ToString())
        {
            return Error.New(new ForbiddenException("This user is not allowed to manage the portfolio"));
        }

        return Unit.Default;
    }
}
