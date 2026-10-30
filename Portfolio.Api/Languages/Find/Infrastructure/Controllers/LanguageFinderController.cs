using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Languages.Find.Application.UseCases;
using Portfolio.Api.Languages.Find.Infrastructure.Dtos;
using Portfolio.Api.Shared.Infrastructure.Ports;

namespace Portfolio.Api.Languages.Find.Infrastructure.Controllers;

[ApiController]
[AllowAnonymous]
[Route("v1/language")]
public class LanguageFinderController(
    LanguageFinder finder,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    [HttpGet]
    [Produces("application/json")]
    public async Task<ActionResult<List<LanguageResponse>>> Find()
    {
        return await (
            from languages in finder.Execute().ToAsync()
            select languages.Select(LanguageResponse.From).ToList()
        ).Match<ActionResult<List<LanguageResponse>>>(
            languages => Ok(languages),
            error => errorResponseMapper.ToActionResult(this, error)
        );
    }
}
