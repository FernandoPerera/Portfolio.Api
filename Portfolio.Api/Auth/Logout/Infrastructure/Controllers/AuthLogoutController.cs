using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Portfolio.Api.Auth.Logout.Application.Dtos;
using Portfolio.Api.Auth.Logout.Application.UseCases;
using Portfolio.Api.Auth.Logout.Infrastructure.Dtos;
using Portfolio.Api.Shared.Infrastructure.Extensions;
using Portfolio.Api.Shared.Infrastructure.Ports;

namespace Portfolio.Api.Auth.Logout.Infrastructure.Controllers;

[ApiController]
[AllowAnonymous]
[EnableRateLimiting(RateLimitingExtensions.AuthPolicy)]
[Route("v1/auth/logout")]
[Tags("Auth")]
public class AuthLogoutController(
    SessionCloser closer,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    /// <summary>
    /// Closes the session of the owner.
    /// </summary>
    /// <remarks>
    /// Sends the refresh token of the session to close. Every refresh token of the owner stops working,
    /// so nobody can renew the session with a copy of them and the owner must sign in with GitHub again.
    /// It works even when the access token has already expired. Closing a session that is already closed,
    /// or with an unknown refresh token, also succeeds. The access token already issued keeps working
    /// until it expires.
    /// </remarks>
    /// <param name="request">The refresh token of the session to close.</param>
    /// <response code="204">The session is closed, or there was no session to close.</response>
    /// <response code="400">The refresh token is empty.</response>
    /// <response code="429">Too many requests from the same address. Wait for the number of seconds in the `Retry-After` header and try again.</response>
    /// <response code="503">The session cannot be closed right now.</response>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult> Logout(LogoutRequest request)
    {
        return await (
            from _ in closer.Execute(new LogoutInfo(request.RefreshToken)).ToAsync()
            select Unit.Default
        ).Match(
            _ => NoContent(),
            error => errorResponseMapper.ToActionResult(this, error)
        );
    }
}
