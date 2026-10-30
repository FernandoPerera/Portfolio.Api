using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Auth.Logout.Application.Dtos;
using Portfolio.Api.Auth.Shared.Domain.Models;
using Portfolio.Api.Auth.Shared.Domain.Ports;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Auth.Logout.Application.UseCases;

public class SessionCloser(
    IRefreshTokenHasher refreshTokenHasher,
    IRefreshTokenFinderRepository refreshTokenFinder,
    IRefreshTokenRevokerRepository refreshTokenRevoker
)
{
    public async Task<Either<Error, Unit>> Execute(LogoutInfo logoutInfo)
    {
        return await (
            from refreshToken in RefreshToken.Create(logoutInfo.RefreshToken).ToAsync()
            from tokenHash in refreshTokenHasher.Hash(refreshToken).ToAsync()
            from result in CloseSession(tokenHash).ToAsync()
            select result
        );
    }

    private async Task<Either<Error, Unit>> CloseSession(RefreshTokenHash tokenHash)
    {
        var found = await refreshTokenFinder.Find(tokenHash);

        return await found.Match(
            stored => refreshTokenRevoker.Revoke(stored),
            error => Task.FromResult(
                error.Exception.Case is UnauthorizedException
                    ? Either<Error, Unit>.Right(Unit.Default)
                    : Either<Error, Unit>.Left(error)
            )
        );
    }
}
