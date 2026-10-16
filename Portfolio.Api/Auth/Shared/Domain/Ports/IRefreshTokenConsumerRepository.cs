using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Auth.Shared.Domain.Models;

namespace Portfolio.Api.Auth.Shared.Domain.Ports;

public interface IRefreshTokenConsumerRepository
{
    Task<Either<Error, Unit>> Consume(StoredRefreshToken consumedRefreshToken);
}
