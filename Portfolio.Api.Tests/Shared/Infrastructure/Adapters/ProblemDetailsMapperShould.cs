using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Portfolio.Api.Auth.Shared.Domain.Errors;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Adapters;
using Portfolio.Api.Tests.Shared.Infrastructure.Builders;
using Shouldly;

namespace Portfolio.Api.Tests.Shared.Infrastructure.Adapters;

public class ProblemDetailsMapperShould
{
    private const string DatabaseFailureMessage = "Database connection timeout";

    private readonly FakeLogger<ProblemDetailsMapper> _logger = new();
    private readonly ProblemDetailsMapper _mapper;
    private readonly TestController _controller = new();

    public ProblemDetailsMapperShould()
    {
        _mapper = ProblemDetailsMapperBuilder.Build(_logger);
    }

    private class TestController : ControllerBase;

    private record UnexpectedError(string Message) : DomainError(Message);

    public static TheoryData<DomainError, int, string, string> ErrorResponses => new()
    {
        { new NotFoundError("Resource not found"), StatusCodes.Status404NotFound, "Not Found", "Resource not found" },
        { new ValidationError("Invalid field format"), StatusCodes.Status400BadRequest, "Bad Request", "Invalid field format" },
        { new UnauthorizedError("Invalid credentials"), StatusCodes.Status401Unauthorized, "Unauthorized", "Invalid credentials" },
        { new RefreshTokenReuseError("Token already used"), StatusCodes.Status401Unauthorized, "Unauthorized", "Token already used" },
        { new ForbiddenError("Not allowed"), StatusCodes.Status403Forbidden, "Forbidden", "Not allowed" },
        { new ConflictError("Language already exists"), StatusCodes.Status409Conflict, "Conflict", "Language already exists" },
        { new ServiceUnavailableError(DatabaseFailureMessage), StatusCodes.Status503ServiceUnavailable, "Service Unavailable", "The service is unavailable right now. Try again later." },
        { new UnexpectedError("Unhandled unexpected error"), StatusCodes.Status500InternalServerError, "Internal Server Error", "An unexpected error occurred." }
    };

    [Theory]
    [MemberData(nameof(ErrorResponses))]
    public void MapTheErrorToItsResponse(DomainError error, int status, string title, string detail)
    {
        var result = _mapper.ToActionResult(_controller, error);

        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(status);
        var problemDetails = objectResult.Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Title.ShouldBe(title);
        problemDetails.Detail.ShouldBe(detail);
        problemDetails.Status.ShouldBe(status);
        problemDetails.Type.ShouldBe($"https://http.cat/{status}");
    }

    [Fact]
    public void LogTheCauseOfAnUnavailableService()
    {
        var cause = new InvalidOperationException("Connection refused");

        var result = _mapper.ToActionResult(_controller, new ServiceUnavailableError(DatabaseFailureMessage, cause));

        var problemDetails = result.ShouldBeOfType<ObjectResult>().Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Detail.ShouldNotBeNull().ShouldNotContain(cause.Message);
        var entry = _logger.Collector.GetSnapshot().ShouldHaveSingleItem();
        entry.Level.ShouldBe(LogLevel.Error);
        entry.Exception.ShouldBe(cause);
    }

    [Fact]
    public void LogAnUnexpectedError()
    {
        _mapper.ToActionResult(_controller, new UnexpectedError("Unhandled unexpected error"));

        var entry = _logger.Collector.GetSnapshot().ShouldHaveSingleItem();
        entry.Level.ShouldBe(LogLevel.Error);
        entry.Message.ShouldContain("Unhandled unexpected error");
    }

    public static TheoryData<DomainError> RejectedErrors => new()
    {
        new UnauthorizedError("Rejected request"),
        new ForbiddenError("Rejected request")
    };

    [Theory]
    [MemberData(nameof(RejectedErrors))]
    public void WarnWhenTheRequestIsRejected(DomainError error)
    {
        _mapper.ToActionResult(_controller, error);

        var entry = _logger.Collector.GetSnapshot().ShouldHaveSingleItem();
        entry.Level.ShouldBe(LogLevel.Warning);
        entry.Message.ShouldContain("Rejected request");
    }

    public static TheoryData<DomainError> ClientErrors => new()
    {
        new NotFoundError("Resource not found"),
        new ValidationError("Invalid field format"),
        new ConflictError("Language already exists")
    };

    [Theory]
    [MemberData(nameof(ClientErrors))]
    public void NotLogTheErrorsOfTheClient(DomainError clientError)
    {
        _mapper.ToActionResult(_controller, clientError);

        _logger.Collector.Count.ShouldBe(0);
    }
}
