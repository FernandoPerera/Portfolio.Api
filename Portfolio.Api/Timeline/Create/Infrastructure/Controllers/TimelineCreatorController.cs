using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Shared.Infrastructure.Ports;
using Portfolio.Api.Timeline.Create.Application.Dtos;
using Portfolio.Api.Timeline.Create.Application.UseCases;
using Portfolio.Api.Timeline.Create.Infrastructure.Dtos;

namespace Portfolio.Api.Timeline.Create.Infrastructure.Controllers;

[ApiController]
[Route("v1/timeline")]
public class TimelineCreatorController(
    TimelineCreator creator,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    [HttpPost]
    [Produces("application/json")]
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
