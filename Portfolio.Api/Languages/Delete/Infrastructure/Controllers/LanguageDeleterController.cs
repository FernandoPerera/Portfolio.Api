using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Languages.Delete.Application.Dtos;
using Portfolio.Api.Languages.Delete.Application.UseCases;
using Portfolio.Api.Shared.Infrastructure.Ports;

namespace Portfolio.Api.Languages.Delete.Infrastructure.Controllers;

[ApiController]
[Authorize]
[Route("v1/language")]
public class LanguageDeleterController(
    LanguageDeleter deleter,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    [HttpDelete("{languageCode}")]
    [Produces("application/json")]
    public async Task<ActionResult> Delete(string languageCode)
    {
        return await (
            from _ in deleter.Execute(new LanguageDeleterInfo(languageCode)).ToAsync()
            select Unit.Default
        ).Match(
            _ => NoContent(),
            error => errorResponseMapper.ToActionResult(this, error)
        );
    }
}
