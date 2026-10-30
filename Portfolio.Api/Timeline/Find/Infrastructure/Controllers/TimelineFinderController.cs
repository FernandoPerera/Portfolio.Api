using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Portfolio.Api.Shared.Infrastructure.Extensions;
using Portfolio.Api.Shared.Infrastructure.Ports;
using Portfolio.Api.Timeline.Find.Application.UseCases;
using Portfolio.Api.Timeline.Find.Infrastructure.Dtos;

namespace Portfolio.Api.Timeline.Find.Infrastructure.Controllers;

[ApiController]
[AllowAnonymous]
[EnableRateLimiting(RateLimitingExtensions.PublicPolicy)]
[Route("v1/timeline")]
public class TimelineFinderController(
    TimelineFinder finder,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    /// <summary>
    /// Lists the career timeline.
    /// </summary>
    /// <remarks>
    /// Returns the work and education entries. Each one has its organization, dates, technologies and the
    /// title and description in every available language.
    /// </remarks>
    /// <response code="200">The timeline entries. The list is empty when none has been created yet.</response>
    /// <response code="429">Too many requests from the same address. Wait for the number of seconds in the `Retry-After` header and try again.</response>
    /// <response code="503">The timeline cannot be read right now.</response>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType<List<TimelineEntryResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<List<TimelineEntryResponse>>> Find()
    {
        return await (
            from entries in finder.Execute().ToAsync()
            select entries.Select(TimelineEntryResponse.From).ToList()
        ).Match<ActionResult<List<TimelineEntryResponse>>>(
            entries => Ok(entries),
            error => errorResponseMapper.ToActionResult(this, error)
        );
    }
}
