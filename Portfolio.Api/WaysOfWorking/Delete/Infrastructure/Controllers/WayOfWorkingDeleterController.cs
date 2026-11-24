using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Shared.Infrastructure.Ports;
using Portfolio.Api.WaysOfWorking.Delete.Application.Dtos;
using Portfolio.Api.WaysOfWorking.Delete.Application.UseCases;

namespace Portfolio.Api.WaysOfWorking.Delete.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("v1/ways-of-working")]
[Tags("Ways of working")]
public class WayOfWorkingDeleterController(
    WayOfWorkingDeleter deleter,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    /// <summary>
    /// Removes a way of working.
    /// </summary>
    /// <remarks>
    /// Deletes the way of working with all its translations. The ones that come after it move up one
    /// position, so the order has no gaps. Only the owner can do it.
    /// </remarks>
    /// <param name="id">Identifier of the way of working.</param>
    /// <response code="204">Way of working removed.</response>
    /// <response code="401">The request has no valid access token. Sign in again or refresh the token.</response>
    /// <response code="403">The signed-in user is not the owner of the portfolio, so they cannot make changes.</response>
    /// <response code="404">The way of working does not exist.</response>
    /// <response code="503">The way of working cannot be removed right now.</response>
    [HttpDelete("{id:guid}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult> Delete(Guid id)
    {
        return await deleter.Execute(new WayOfWorkingDeleterInfo(id))
            .ToAsync()
            .Match(
                _ => NoContent(),
                error => errorResponseMapper.ToActionResult(this, error)
            );
    }
}
