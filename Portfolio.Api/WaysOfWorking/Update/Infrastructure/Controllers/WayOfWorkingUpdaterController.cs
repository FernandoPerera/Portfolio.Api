using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Shared.Infrastructure.Ports;
using Portfolio.Api.WaysOfWorking.Update.Application.Dtos;
using Portfolio.Api.WaysOfWorking.Update.Application.UseCases;
using Portfolio.Api.WaysOfWorking.Update.Infrastructure.Dtos;

namespace Portfolio.Api.WaysOfWorking.Update.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("v1/ways-of-working")]
[Tags("Ways of working")]
public class WayOfWorkingUpdaterController(
    WayOfWorkingUpdater updater,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    /// <summary>
    /// Changes the illustration of a way of working.
    /// </summary>
    /// <remarks>
    /// The illustration is shared by all languages. The texts of each language are edited with the
    /// translation endpoint. Only the owner can do it.
    /// </remarks>
    /// <param name="id">Identifier of the way of working.</param>
    /// <param name="request">The new illustration: a key in lowercase letters, digits and hyphens between words (up to 30 characters).</param>
    /// <response code="204">Way of working updated.</response>
    /// <response code="400">The illustration is empty, too long or does not have a valid format.</response>
    /// <response code="401">The request has no valid access token. Sign in again or refresh the token.</response>
    /// <response code="403">The signed-in user is not the owner of the portfolio, so they cannot make changes.</response>
    /// <response code="404">The way of working does not exist.</response>
    /// <response code="503">The way of working cannot be saved right now.</response>
    [HttpPatch("{id:guid}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult> Update(Guid id, UpdateWayOfWorkingRequest request)
    {
        return await updater.Execute(new WayOfWorkingUpdaterInfo(id, request.Illustration))
            .ToAsync()
            .Match(
                _ => NoContent(),
                error => errorResponseMapper.ToActionResult(this, error)
            );
    }
}
