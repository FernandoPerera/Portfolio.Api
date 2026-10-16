using LanguageExt;
using LanguageExt.Common;

namespace Portfolio.Api.Auth.Shared.Domain.Ports;

public interface IExpiredRefreshTokenCleanerRepository
{
    Task<Either<Error, Unit>> Clean();
}
