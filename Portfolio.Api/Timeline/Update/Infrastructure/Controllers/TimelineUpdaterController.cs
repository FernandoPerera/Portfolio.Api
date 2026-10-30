using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Shared.Infrastructure.Ports;
using Portfolio.Api.Timeline.Update.Application.Dtos;
using Portfolio.Api.Timeline.Update.Application.UseCases;
using Portfolio.Api.Timeline.Update.Infrastructure.Dtos;

namespace Portfolio.Api.Timeline.Update.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("v1/timeline")]
public class TimelineUpdaterController(
    TimelineUpdater updater,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    /// <summary>
    /// Edits a timeline entry in one language.
    /// </summary>
    /// <remarks>
    /// Changes only the fields that are sent. The organization, category, dates and technologies are shared
    /// by all languages, while the title and the description belong to the language in the address. Only
    /// the owner can do it.
    /// </remarks>
    /// <param name="id">Identifier of the timeline entry.</param>
    /// <param name="languageCode">Two-letter code of the language whose title and description are edited, for example `es`.</param>
    /// <param name="request">The fields to change. The ones left out stay as they are.</param>
    /// <response code="204">Entry updated.</response>
    /// <response code="400">A sent field is empty or too long, the category or a date is not valid, or a technology is repeated.</response>
    /// <response code="401">The request has no valid access token. Sign in again or refresh the token.</response>
    /// <response code="403">The signed-in user is not the owner of the portfolio, so they cannot make changes.</response>
    /// <response code="404">The entry does not exist, or it has no translation in that language.</response>
    /// <response code="503">The entry cannot be saved right now.</response>
    [HttpPatch("{id:guid}/{languageCode}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult> Update(Guid id, string languageCode, UpdateTimelineRequest request)
    {
        return await updater.Execute(
                new TimelineUpdaterInfo(
                    id,
                    request.Organization,
                    request.Category,
                    request.StartDate,
                    request.EndDate,
                    request.Technologies,
                    request.Title,
                    request.Description,
                    languageCode
                )
            )
            .ToAsync()
            .Match(
                _ => NoContent(),
                error => errorResponseMapper.ToActionResult(this, error)
            );
    }
}
