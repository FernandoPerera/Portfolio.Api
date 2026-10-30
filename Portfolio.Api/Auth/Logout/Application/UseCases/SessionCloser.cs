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

    private async Task<Either<Error, Unit>> CloseSession(RefreshTokenHash tokenHash) =>
        await (await refreshTokenFinder.Find(tokenHash))
            .Match(
                Right: refreshTokenRevoker.Revoke,
                Left: error => Task.FromResult<Either<Error, Unit>>(
                    error.Exception.Case is UnauthorizedException
                        ? Unit.Default
                        : error
                )
            );
}