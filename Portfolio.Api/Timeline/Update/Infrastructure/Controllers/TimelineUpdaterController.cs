using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Shared.Infrastructure.Ports;
using Portfolio.Api.Timeline.Create.Application.Dtos;
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
    [HttpPatch("{id:guid}")]
    [Produces("application/json")]
    public async Task<ActionResult> Update(Guid id, UpdateTimelineRequest request)
    {
        return await updater.Execute(
                new TimelineUpdaterInfo(
                    id,
                    request.Organization,
                    request.Category,
                    request.StartDate,
                    request.EndDate,
                    request.Technologies,
                    request.Translations?
                        .Select(translation => new TimelineTranslationInfo(
                            translation.LanguageCode,
                            translation.Title,
                            translation.Description
                        ))
                        .ToList()
                )
            )
            .ToAsync()
            .Match(
                _ => NoContent(),
                error => errorResponseMapper.ToActionResult(this, error)
            );
    }
}
