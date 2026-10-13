using LanguageExt.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Adapters;
using Shouldly;

namespace Portfolio.Api.Tests.Shared.Infrastructure.Adapters;

public class ProblemDetailsMapperShould
{
    private readonly ProblemDetailsMapper _mapper = new();
    private readonly TestController _controller = new();

    private class TestController : ControllerBase;

    [Fact]
    public void MapToNotFound()
    {
        var error = Error.New(new NotFoundException("Resource not found"));

        var result = _mapper.ToActionResult(_controller, error);

        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
        var problemDetails = objectResult.Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Title.ShouldBe("Not Found");
        problemDetails.Detail.ShouldBe("Resource not found");
        problemDetails.Status.ShouldBe(StatusCodes.Status404NotFound);
        problemDetails.Type.ShouldBe("https://http.cat/404");
    }

    [Fact]
    public void MapToBadRequest()
    {
        var error = Error.New(new ValidationException("Invalid field format"));

        var result = _mapper.ToActionResult(_controller, error);

        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        var problemDetails = objectResult.Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Title.ShouldBe("Bad Request");
        problemDetails.Detail.ShouldBe("Invalid field format");
        problemDetails.Status.ShouldBe(StatusCodes.Status400BadRequest);
        problemDetails.Type.ShouldBe("https://http.cat/400");
    }

    [Fact]
    public void MapToForbidden()
    {
        var error = Error.New(new ForbiddenException("Not allowed"));

        var result = _mapper.ToActionResult(_controller, error);

        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(StatusCodes.Status403Forbidden);
        var problemDetails = objectResult.Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Title.ShouldBe("Forbidden");
        problemDetails.Detail.ShouldBe("Not allowed");
        problemDetails.Status.ShouldBe(StatusCodes.Status403Forbidden);
        problemDetails.Type.ShouldBe("https://http.cat/403");
    }

    [Fact]
    public void MapToUnauthorized()
    {
        var error = Error.New(new UnauthorizedException("Invalid credentials"));

        var result = _mapper.ToActionResult(_controller, error);

        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(StatusCodes.Status401Unauthorized);
        var problemDetails = objectResult.Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Title.ShouldBe("Unauthorized");
        problemDetails.Detail.ShouldBe("Invalid credentials");
        problemDetails.Status.ShouldBe(StatusCodes.Status401Unauthorized);
        problemDetails.Type.ShouldBe("https://http.cat/401");
    }

    [Fact]
    public void MapToConflict()
    {
        var error = Error.New(new ConflictException("Language already exists"));

        var result = _mapper.ToActionResult(_controller, error);

        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(StatusCodes.Status409Conflict);
        var problemDetails = objectResult.Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Title.ShouldBe("Conflict");
        problemDetails.Detail.ShouldBe("Language already exists");
        problemDetails.Status.ShouldBe(StatusCodes.Status409Conflict);
        problemDetails.Type.ShouldBe("https://http.cat/409");
    }

    [Fact]
    public void MapToServiceUnavailable()
    {
        var error = Error.New(new ServiceUnavailableException("Database connection timeout"));

        var result = _mapper.ToActionResult(_controller, error);

        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(StatusCodes.Status503ServiceUnavailable);
        var problemDetails = objectResult.Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Title.ShouldBe("Service Unavailable");
        problemDetails.Detail.ShouldBe("Database connection timeout");
        problemDetails.Status.ShouldBe(StatusCodes.Status503ServiceUnavailable);
        problemDetails.Type.ShouldBe("https://http.cat/503");
    }

    [Fact]
    public void MapToInternalServerError()
    {
        var error = Error.New(new Exception("Unhandled unexpected error"));

        var result = _mapper.ToActionResult(_controller, error);

        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
        var problemDetails = objectResult.Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Title.ShouldBe("Internal Server Error");
        problemDetails.Detail.ShouldBe("Unhandled unexpected error");
        problemDetails.Status.ShouldBe(StatusCodes.Status500InternalServerError);
        problemDetails.Type.ShouldBe("https://http.cat/500");
    }
}
