using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Portfolio.Api.Auth.Refresh.Application.Dtos;
using Portfolio.Api.Auth.Refresh.Application.UseCases;
using Portfolio.Api.Auth.Refresh.Infrastructure.Dtos;
using Portfolio.Api.Auth.Shared.Infrastructure.Dtos;
using Portfolio.Api.Shared.Infrastructure.Extensions;
using Portfolio.Api.Shared.Infrastructure.Ports;

namespace Portfolio.Api.Auth.Refresh.Infrastructure.Controllers;

[ApiController]
[AllowAnonymous]
[EnableRateLimiting(RateLimitingExtensions.AuthPolicy)]
[Route("v1/auth/refresh")]
public class AuthRefreshController(
    TokenRefresher refresher,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    /// <summary>
    /// Renews the session without signing in again.
    /// </summary>
    /// <remarks>
    /// Swaps the refresh token for a new access token and a new refresh token. Each refresh token works
    /// only once, so the old one stops working. If a token that was already used comes back, the API
    /// assumes it was copied, closes the whole session and the owner must sign in with GitHub again.
    /// </remarks>
    /// <param name="request">The refresh token received when signing in or in the last refresh.</param>
    /// <response code="200">New pair of tokens. Keep the new refresh token, the old one no longer works.</response>
    /// <response code="400">The refresh token is empty.</response>
    /// <response code="401">The refresh token is unknown, has expired or was already used. In the last case the whole session is closed.</response>
    /// <response code="429">Too many requests from the same address. Wait for the number of seconds in the `Retry-After` header and try again.</response>
    /// <response code="503">The tokens cannot be renewed right now.</response>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType<TokensResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<TokensResponse>> Refresh(RefreshRequest request)
    {
        return await (
            from tokens in refresher.Execute(new RefreshInfo(request.RefreshToken)).ToAsync()
            select TokensResponse.From(tokens)
        ).Match<ActionResult<TokensResponse>>(
            tokens => Ok(tokens),
            error => errorResponseMapper.ToActionResult(this, error)
        );
    }
}
