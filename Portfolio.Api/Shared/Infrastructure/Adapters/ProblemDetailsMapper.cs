using LanguageExt.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Ports;

namespace Portfolio.Api.Shared.Infrastructure.Adapters;

public class ProblemDetailsMapper : IErrorResponseMapper
{
    public ActionResult ToActionResult(ControllerBase controller, Error error)
    {
        return error.Exception.Case switch
        {
            NotFoundException => CreateProblemResult(
                controller,
                StatusCodes.Status404NotFound,
                "Not Found",
                "https://http.cat/404",
                error.Message
            ),
            ValidationException => CreateProblemResult(
                controller,
                StatusCodes.Status400BadRequest,
                "Bad Request",
                "https://http.cat/400",
                error.Message
            ),
            ConflictException => CreateProblemResult(
                controller,
                StatusCodes.Status409Conflict,
                "Conflict",
                "https://http.cat/409",
                error.Message
            ),
            ServiceUnavailableException => CreateProblemResult(
                controller,
                StatusCodes.Status503ServiceUnavailable,
                "Service Unavailable",
                "https://http.cat/503",
                error.Message
            ),
            _ => CreateProblemResult(
                controller,
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "https://http.cat/500",
                error.Message
            )
        };
    }

    private static ObjectResult CreateProblemResult(
        ControllerBase controller,
        int statusCode,
        string title,
        string type,
        string detail
    )
    {
        return new ObjectResult(new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Type = type,
            Detail = detail,
            Instance = controller.ControllerContext?.HttpContext?.Request?.Path
        })
        {
            StatusCode = statusCode
        };
    }
}
