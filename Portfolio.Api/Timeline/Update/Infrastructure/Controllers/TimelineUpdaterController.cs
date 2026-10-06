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
    [HttpPatch("{id:guid}/{languageCode}")]
    [Produces("application/json")]
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
