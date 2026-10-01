using LanguageExt;
using LanguageExt.Common;
using NSubstitute;
using Portfolio.Api.Profiles.Find.Application.UseCases;
using Portfolio.Api.Profiles.Find.Domain.Ports;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Tests.Profiles.Find.Domain.Builders;
using Shouldly;

namespace Portfolio.Api.Tests.Profiles.Find.Application.UseCases;

public class ProfileFinderShould
{
    private readonly IProfileFinderRepository _repository;
    private readonly ProfileFinder _finder;

    public ProfileFinderShould()
    {
        _repository = Substitute.For<IProfileFinderRepository>();
        _finder = new ProfileFinder(_repository);
    }

    [Fact]
    public async Task ReturnProfileInfoWhenProfileExists()
    {
        var domainProfile = ProfileFoundBuilder.Build();
        _repository.FindProfile().Returns(domainProfile);

        var result = await _finder.Execute();

        result.Match(
            profile => profile.ShouldBe(domainProfile),
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task ReturnRepositoryErrorWhenProfileRetrievalFails()
    {
        var expectedError = Error.New(new ServiceUnavailableException("Database unavailable"));
        _repository.FindProfile().Returns(expectedError);

        var result = await _finder.Execute();

        result.Match(
            info => throw new InvalidOperationException("Should have failed"),
            error => error.ShouldBe(expectedError)
        );
    }
}
