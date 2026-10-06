using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Languages.Update.Application.Dtos;
using Portfolio.Api.Languages.Update.Application.UseCases;
using Portfolio.Api.Languages.Update.Infrastructure.Dtos;
using Portfolio.Api.Shared.Infrastructure.Ports;

namespace Portfolio.Api.Languages.Update.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("v1/language")]
public class LanguageUpdaterController(
    LanguageUpdater updater,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    [HttpPatch("{languageCode}")]
    [Produces("application/json")]
    public async Task<ActionResult> Update(string languageCode, UpdateLanguageRequest request)
    {
        return await (
            from _ in updater.Execute(
                new LanguageUpdaterInfo(languageCode, request.Name)
            ).ToAsync()
            select Unit.Default
        ).Match(
            _ => NoContent(),
            error => errorResponseMapper.ToActionResult(this, error)
        );
    }
}
