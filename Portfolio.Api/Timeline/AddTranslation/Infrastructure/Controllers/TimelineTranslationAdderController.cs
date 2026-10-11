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
    [HttpPost("{id:guid}/{languageCode}")]
    [Produces("application/json")]
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
