using LanguageExt;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Auth.Shared.Domain.Ports;

public interface IRefreshTokenConsumerRepository
{
    Task<Either<DomainError, Unit>> Consume(StoredRefreshToken consumedRefreshToken);
}
