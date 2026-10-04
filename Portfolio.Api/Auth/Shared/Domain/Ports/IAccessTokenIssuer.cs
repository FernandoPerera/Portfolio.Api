using LanguageExt.Common;
using LanguageExt;
using Portfolio.Api.Auth.Shared.Domain.Models;

namespace Portfolio.Api.Auth.Shared.Domain.Ports;

public interface IAccessTokenIssuer
{
    Either<Error, AccessToken> Issue(AdminUsername username);
}
