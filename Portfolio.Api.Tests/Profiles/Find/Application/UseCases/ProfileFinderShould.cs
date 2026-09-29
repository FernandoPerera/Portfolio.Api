using LanguageExt;
using LanguageExt.Common;
using NSubstitute;
using Portfolio.Api.Profiles.Find.Application.Dtos;
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
        var query = new ProfileFinderInfo(Language: "es");
        var domainProfile = ProfileFoundBuilder.Build(
            name: "Fernando",
            jobRole: "Beer software architect",
            presentation: "Mucho gusto :)",
            enterprise: "Lean Mind"
        );
        _repository.FindProfile().Returns(domainProfile);

        var result = await _finder.Execute(query);

        result.Match(
            profile =>
            {
                profile.GetName().ShouldBe("Fernando");
                profile.GetRole().ShouldBe("Beer software architect");
                profile.GetPresentation().ShouldBe("Mucho gusto :)");
                profile.GetEnterprise().ShouldBe("Lean Mind");
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task ReturnRepositoryErrorWhenProfileRetrievalFails()
    {
        var query = new ProfileFinderInfo(Language: "es");
        var expectedError = Error.New(new ServiceUnavailableException("Database unavailable"));
        _repository.FindProfile().Returns(expectedError);

        var result = await _finder.Execute(query);

        result.Match(
            info => throw new InvalidOperationException("Should have failed"),
            error => error.ShouldBe(expectedError)
        );
    }
}
