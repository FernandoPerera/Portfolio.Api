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
    /// <summary>
    /// Removes a language.
    /// </summary>
    /// <remarks>
    /// Deletes a language that nobody uses. A language that already has profile or timeline translations
    /// cannot be removed until those translations are gone. Only the owner can do it.
    /// </remarks>
    /// <param name="languageCode">Two-letter code of the language to remove, for example `en`.</param>
    /// <response code="204">Language removed.</response>
    /// <response code="400">The code does not have two letters.</response>
    /// <response code="401">The request has no valid access token. Sign in again or refresh the token.</response>
    /// <response code="403">The signed-in user is not the owner of the portfolio, so they cannot make changes.</response>
    /// <response code="404">There is no language with that code.</response>
    /// <response code="409">The language is used by existing translations, so it cannot be removed.</response>
    /// <response code="503">The language cannot be removed right now.</response>
    [HttpDelete("{languageCode}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
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
