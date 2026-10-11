using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Shared.Infrastructure.Ports;
using Portfolio.Api.Timeline.Find.Application.UseCases;
using Portfolio.Api.Timeline.Find.Infrastructure.Dtos;

namespace Portfolio.Api.Timeline.Find.Infrastructure.Controllers;

[ApiController]
[AllowAnonymous]
[Route("v1/timeline")]
public class TimelineFinderController(
    TimelineFinder finder,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    [HttpGet]
    [Produces("application/json")]
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
