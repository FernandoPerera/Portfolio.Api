using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Shared.Infrastructure.Ports;
using Portfolio.Api.Timeline.AddTranslation.Application.Dtos;
using Portfolio.Api.Timeline.AddTranslation.Application.UseCases;
using Portfolio.Api.Timeline.AddTranslation.Infrastructure.Dtos;

namespace Portfolio.Api.Timeline.AddTranslation.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("v1/timeline")]
public class TimelineTranslationAdderController(
    TimelineTranslationAdder adder,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    /// <summary>
    /// Adds a translation to a timeline entry.
    /// </summary>
    /// <remarks>
    /// Writes the title and the description of an existing entry in one more language. The language must
    /// exist and the entry must not have a translation in it yet. Only the owner can do it.
    /// </remarks>
    /// <param name="id">Identifier of the timeline entry.</param>
    /// <param name="languageCode">Two-letter code of the language of the new translation, for example `en`.</param>
    /// <param name="request">The title (up to 100 characters) and the description (up to 1000 characters) in that language.</param>
    /// <response code="201">Translation added.</response>
    /// <response code="400">The title or the description is empty or too long, or the language code is not valid.</response>
    /// <response code="401">The request has no valid access token. Sign in again or refresh the token.</response>
    /// <response code="403">The signed-in user is not the owner of the portfolio, so they cannot make changes.</response>
    /// <response code="404">The entry or the language does not exist.</response>
    /// <response code="409">The entry already has a translation in that language.</response>
    /// <response code="503">The translation cannot be saved right now.</response>
    [HttpPost("{id:guid}/{languageCode}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult> Add(Guid id, string languageCode, AddTimelineTranslationRequest request)
    {
        return await (
            from _ in adder.Execute(
                new TimelineTranslationAdderInfo(id, languageCode, request.Title, request.Description)
            ).ToAsync()
            select Unit.Default
        ).Match(
            _ => CreatedAtAction(nameof(Add), request),
            error => errorResponseMapper.ToActionResult(this, error)
        );
    }
}
