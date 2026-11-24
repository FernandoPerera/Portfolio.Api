using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Portfolio.Api.Shared.Infrastructure.Extensions;
using Portfolio.Api.Shared.Infrastructure.Ports;
using Portfolio.Api.WaysOfWorking.Find.Application.UseCases;
using Portfolio.Api.WaysOfWorking.Find.Infrastructure.Dtos;

namespace Portfolio.Api.WaysOfWorking.Find.Infrastructure.Controllers;

[ApiController]
[AllowAnonymous]
[EnableRateLimiting(RateLimitingExtensions.PublicPolicy)]
[Route("v1/ways-of-working")]
[Tags("Ways of working")]
public class WayOfWorkingFinderController(
    WayOfWorkingFinder finder,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    /// <summary>
    /// Lists the ways of working.
    /// </summary>
    /// <remarks>
    /// Returns the ways of working in the order they are shown, from the first position. Each one has its
    /// illustration key and the label, caption, title, description and points in every available language.
    /// </remarks>
    /// <response code="200">The ways of working. The list is empty when none has been created yet.</response>
    /// <response code="429">Too many requests from the same address. Wait for the number of seconds in the `Retry-After` header and try again.</response>
    /// <response code="503">The ways of working cannot be read right now.</response>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType<List<WayOfWorkingResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<List<WayOfWorkingResponse>>> Find()
    {
        return await (
            from ways in finder.Execute().ToAsync()
            select ways.Select(WayOfWorkingResponse.From).ToList()
        ).Match<ActionResult<List<WayOfWorkingResponse>>>(
            ways => Ok(ways),
            error => errorResponseMapper.ToActionResult(this, error)
        );
    }
}
