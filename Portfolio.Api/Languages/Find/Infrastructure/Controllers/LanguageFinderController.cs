using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Portfolio.Api.Languages.Find.Application.UseCases;
using Portfolio.Api.Languages.Find.Infrastructure.Dtos;
using Portfolio.Api.Shared.Infrastructure.Extensions;
using Portfolio.Api.Shared.Infrastructure.Ports;

namespace Portfolio.Api.Languages.Find.Infrastructure.Controllers;

[ApiController]
[AllowAnonymous]
[EnableRateLimiting(RateLimitingExtensions.PublicPolicy)]
[Route("v1/language")]
public class LanguageFinderController(
    LanguageFinder finder,
    IErrorResponseMapper errorResponseMapper
) : ControllerBase
{
    /// <summary>
    /// Lists the available languages.
    /// </summary>
    /// <remarks>
    /// Returns every language in which the portfolio content can be translated. The language codes are the
    /// ones used to read and write translations.
    /// </remarks>
    /// <response code="200">The list of languages. It is empty when none has been created yet.</response>
    /// <response code="429">Too many requests from the same address. Wait for the number of seconds in the `Retry-After` header and try again.</response>
    /// <response code="503">The languages cannot be read right now.</response>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType<List<LanguageResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
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
