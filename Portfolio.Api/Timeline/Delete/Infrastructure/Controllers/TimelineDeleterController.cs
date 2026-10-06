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
public class TimelineDeleterController(
    TimelineDeleter deleter,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    [HttpDelete("{id:guid}")]
    [Produces("application/json")]
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
