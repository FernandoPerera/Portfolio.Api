using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Auth.Shared.Domain.Ports;

public interface IExpiredRefreshTokenCleanerRepository
{
    Task<Either<DomainError, Unit>> Clean();
}
