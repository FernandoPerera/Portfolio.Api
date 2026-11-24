using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Shared.Infrastructure.Ports;
using Portfolio.Api.WaysOfWorking.Reorder.Application.Dtos;
using Portfolio.Api.WaysOfWorking.Reorder.Application.UseCases;
using Portfolio.Api.WaysOfWorking.Reorder.Infrastructure.Dtos;

namespace Portfolio.Api.WaysOfWorking.Reorder.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("v1/ways-of-working")]
[Tags("Ways of working")]
public class WayOrderUpdaterController(
    WayOrderUpdater updater,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    /// <summary>
    /// Changes the order of the ways of working.
    /// </summary>
    /// <remarks>
    /// Receives the identifiers of all the ways of working in the order they must be shown. The list must
    /// contain every existing way of working exactly once: none can be missing, unknown or repeated. Only
    /// the owner can do it.
    /// </remarks>
    /// <param name="request">The identifiers of all the ways of working, first to last.</param>
    /// <response code="204">Order updated.</response>
    /// <response code="400">The list is empty, repeats an identifier, or does not contain exactly the existing ways of working.</response>
    /// <response code="401">The request has no valid access token. Sign in again or refresh the token.</response>
    /// <response code="403">The signed-in user is not the owner of the portfolio, so they cannot make changes.</response>
    /// <response code="503">The order cannot be saved right now.</response>
    [HttpPut("order")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult> Reorder(ReorderWaysOfWorkingRequest request)
    {
        return await updater.Execute(new WayOrderUpdaterInfo(request.Ids))
            .ToAsync()
            .Match(
                _ => NoContent(),
                error => errorResponseMapper.ToActionResult(this, error)
            );
    }
}
