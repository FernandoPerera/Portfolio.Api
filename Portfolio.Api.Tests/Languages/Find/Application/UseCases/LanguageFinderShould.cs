using NSubstitute;
using Portfolio.Api.Languages.Find.Application.UseCases;
using Portfolio.Api.Languages.Find.Domain.Models;
using Portfolio.Api.Languages.Find.Domain.Ports;
using Portfolio.Api.Tests.Languages.Find.Domain.Builders;
using Shouldly;

namespace Portfolio.Api.Tests.Languages.Find.Application.UseCases;

public class LanguageFinderShould
{
    private readonly ILanguageFinderRepository _repository;
    private readonly LanguageFinder _finder;

    public LanguageFinderShould()
    {
        _repository = Substitute.For<ILanguageFinderRepository>();
        _finder = new LanguageFinder(_repository);
    }

    [Fact]
    public async Task FindLanguages()
    {
        var languages = new List<LanguageFound>
        {
            LanguageFoundBuilder.Build("en", "English"),
            LanguageFoundBuilder.Build("es", "Español")
        };
        _repository.FindLanguages().Returns(languages);

        var result = await _finder.Execute();

        result.Match(
            found => found.ShouldBe(languages),
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task FindNoLanguagesWhenThereAreNone()
    {
        _repository.FindLanguages().Returns(new List<LanguageFound>());

        var result = await _finder.Execute();

        result.Match(
            found => found.ShouldBeEmpty(),
            error => error.ShouldBeNull()
        );
    }
}
