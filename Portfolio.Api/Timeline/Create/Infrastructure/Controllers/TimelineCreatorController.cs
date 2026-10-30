using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Shared.Infrastructure.Ports;
using Portfolio.Api.Timeline.Create.Application.Dtos;
using Portfolio.Api.Timeline.Create.Application.UseCases;
using Portfolio.Api.Timeline.Create.Infrastructure.Dtos;

namespace Portfolio.Api.Timeline.Create.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("v1/timeline")]
[Tags("Timeline")]
public class TimelineCreatorController(
    TimelineCreator creator,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    /// <summary>
    /// Adds an entry to the timeline.
    /// </summary>
    /// <remarks>
    /// Creates a work or education entry with at least one translation. The category is `work` or
    /// `education`. Dates have the format `yyyy-MM`. The end date is optional, for an entry that is still
    /// going on, and cannot be earlier than the start date. The technologies are optional and cannot
    /// repeat. Every language must exist and cannot be used twice in the same entry. Only the owner can do
    /// it.
    /// </remarks>
    /// <param name="request">The organization, category, dates, technologies and the translations of the entry.</param>
    /// <response code="201">Entry created.</response>
    /// <response code="400">A required field is empty, a text is too long, the category or a date is not valid, or a technology or a language is repeated.</response>
    /// <response code="401">The request has no valid access token. Sign in again or refresh the token.</response>
    /// <response code="403">The signed-in user is not the owner of the portfolio, so they cannot make changes.</response>
    /// <response code="404">One of the languages does not exist. Create it first.</response>
    /// <response code="503">The entry cannot be saved right now.</response>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult> Create(CreateTimelineRequest request)
    {
        return await (
            from _ in creator.Execute(
                new TimelineCreatorInfo(
                    request.Organization,
                    request.Category,
                    request.StartDate,
                    request.EndDate,
                    request.Technologies ?? [],
                    request.Translations
                        .Select(translation => new TimelineTranslationInfo(
                            translation.LanguageCode,
                            translation.Title,
                            translation.Description
                        ))
                        .ToList()
                )
            ).ToAsync()
            select Unit.Default
        ).Match(
            _ => CreatedAtAction(nameof(Create), request),
            error => errorResponseMapper.ToActionResult(this, error)
        );
    }
}
