using LanguageExt.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Ports;

namespace Portfolio.Api.Shared.Infrastructure.Adapters;

public class ProblemDetailsMapper(ILogger<ProblemDetailsMapper> logger) : IErrorResponseMapper
{
    private const string ServiceUnavailableDetail = "The service is unavailable right now. Try again later.";
    private const string UnexpectedErrorDetail = "An unexpected error occurred.";

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
            UnauthorizedException => CreateRejectedResult(
                controller,
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                "https://http.cat/401",
                error.Message
            ),
            ForbiddenException => CreateRejectedResult(
                controller,
                StatusCodes.Status403Forbidden,
                "Forbidden",
                "https://http.cat/403",
                error.Message
            ),
            ConflictException => CreateProblemResult(
                controller,
                StatusCodes.Status409Conflict,
                "Conflict",
                "https://http.cat/409",
                error.Message
            ),
            ServiceUnavailableException => CreateServerErrorResult(
                controller,
                StatusCodes.Status503ServiceUnavailable,
                "Service Unavailable",
                "https://http.cat/503",
                ServiceUnavailableDetail,
                error.Message,
                error.ToException()
            ),
            _ => CreateServerErrorResult(
                controller,
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "https://http.cat/500",
                UnexpectedErrorDetail,
                error.Message,
                error.ToException()
            )
        };
    }

    public ActionResult ToActionResult(ControllerBase controller, DomainError error)
    {
        return error switch
        {
            NotFoundError => CreateProblemResult(
                controller,
                StatusCodes.Status404NotFound,
                "Not Found",
                "https://http.cat/404",
                error.Message
            ),
            ValidationError => CreateProblemResult(
                controller,
                StatusCodes.Status400BadRequest,
                "Bad Request",
                "https://http.cat/400",
                error.Message
            ),
            UnauthorizedError => CreateRejectedResult(
                controller,
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                "https://http.cat/401",
                error.Message
            ),
            ForbiddenError => CreateRejectedResult(
                controller,
                StatusCodes.Status403Forbidden,
                "Forbidden",
                "https://http.cat/403",
                error.Message
            ),
            ConflictError => CreateProblemResult(
                controller,
                StatusCodes.Status409Conflict,
                "Conflict",
                "https://http.cat/409",
                error.Message
            ),
            ServiceUnavailableError unavailable => CreateServerErrorResult(
                controller,
                StatusCodes.Status503ServiceUnavailable,
                "Service Unavailable",
                "https://http.cat/503",
                ServiceUnavailableDetail,
                error.Message,
                unavailable.Cause
            ),
            _ => CreateServerErrorResult(
                controller,
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "https://http.cat/500",
                UnexpectedErrorDetail,
                error.Message,
                null
            )
        };
    }

    private ObjectResult CreateRejectedResult(
        ControllerBase controller,
        int statusCode,
        string title,
        string type,
        string message
    )
    {
        logger.LogWarning(
            "Request to {Path} rejected with {StatusCode}: {Message}",
            GetPath(controller),
            statusCode,
            message
        );

        return CreateProblemResult(controller, statusCode, title, type, message);
    }

    private ObjectResult CreateServerErrorResult(
        ControllerBase controller,
        int statusCode,
        string title,
        string type,
        string detail,
        string message,
        Exception? cause
    )
    {
        logger.LogError(
            cause,
            "Request to {Path} failed with {StatusCode}: {Message}",
            GetPath(controller),
            statusCode,
            message
        );

        return CreateProblemResult(controller, statusCode, title, type, detail);
    }

    private static PathString? GetPath(ControllerBase controller) =>
        controller.ControllerContext?.HttpContext?.Request?.Path;

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
            Instance = GetPath(controller)
        })
        {
            StatusCode = statusCode
        };
    }
}
