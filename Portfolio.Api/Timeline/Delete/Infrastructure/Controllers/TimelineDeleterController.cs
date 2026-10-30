using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Shared.Infrastructure.Ports;
using Portfolio.Api.Timeline.Delete.Application.Dtos;
using Portfolio.Api.Timeline.Delete.Application.UseCases;

namespace Portfolio.Api.Timeline.Delete.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("v1/timeline")]
[Tags("Timeline")]
public class TimelineDeleterController(
    TimelineDeleter deleter,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    /// <summary>
    /// Removes a timeline entry.
    /// </summary>
    /// <remarks>
    /// Deletes the entry with all its translations. Only the owner can do it.
    /// </remarks>
    /// <param name="id">Identifier of the timeline entry.</param>
    /// <response code="204">Entry removed.</response>
    /// <response code="401">The request has no valid access token. Sign in again or refresh the token.</response>
    /// <response code="403">The signed-in user is not the owner of the portfolio, so they cannot make changes.</response>
    /// <response code="404">The entry does not exist.</response>
    /// <response code="503">The entry cannot be removed right now.</response>
    [HttpDelete("{id:guid}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult> Delete(Guid id)
    {
        return await (
            from _ in deleter.Execute(new TimelineDeleterInfo(id)).ToAsync()
            select Unit.Default
        ).Match(
            _ => NoContent(),
            error => errorResponseMapper.ToActionResult(this, error)
        );
    }
}
