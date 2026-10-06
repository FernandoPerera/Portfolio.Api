using LanguageExt;
using LanguageExt.Common;
using NSubstitute;
using Portfolio.Api.Languages.Update.Application.Dtos;
using Portfolio.Api.Languages.Update.Application.UseCases;
using Portfolio.Api.Languages.Update.Domain.Models;
using Portfolio.Api.Languages.Update.Domain.Ports;
using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Profiles.Shared.Domain.Ports;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Tests.Shared.Extensions;
using Shouldly;

namespace Portfolio.Api.Tests.Languages.Update.Application.UseCases;

public class LanguageUpdaterShould
{
    private readonly ILanguageExistenceRepository _languageExistenceRepository = Substitute.For<ILanguageExistenceRepository>();
    private readonly ILanguageUpdaterRepository _updaterRepository = Substitute.For<ILanguageUpdaterRepository>();
    private readonly LanguageUpdater _updater;

    public LanguageUpdaterShould()
    {
        _updater = new LanguageUpdater(_languageExistenceRepository, _updaterRepository);
        _languageExistenceRepository.Exists(Arg.Any<LanguageCode>()).Returns(Unit.Default);
        _updaterRepository.UpdateLanguage(Arg.Any<LanguageToBeUpdated>()).Returns(Unit.Default);
    }

    [Fact]
    public async Task UpdateLanguage()
    {
        const string givenLanguageCode = "es";
        const string givenName = "Español";
        var updaterInfo = new LanguageUpdaterInfo(givenLanguageCode, givenName);

        var result = await _updater.Execute(updaterInfo);

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                _updaterRepository.Received(1).UpdateLanguage(Arg.Is<LanguageToBeUpdated>(language =>
                    language.GetLanguageCode() == givenLanguageCode && language.GetName() == givenName
                ));
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task FailWhenLanguageIsInvalid()
    {
        var updaterInfo = new LanguageUpdaterInfo("esp", "Español");

        var result = await _updater.Execute(updaterInfo);

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error =>
            {
                error.Exception.Case.ShouldBeOfType<ValidationException>();
                _updaterRepository.DidNotReceive().UpdateLanguage(Arg.Any<LanguageToBeUpdated>());
            }
        );
    }

    [Fact]
    public async Task FailWhenLanguageDoesNotExist()
    {
        var updaterInfo = new LanguageUpdaterInfo("fr", "Français");
        _languageExistenceRepository.Exists(Arg.Any<LanguageCode>())
            .Returns(Error.New(new NotFoundException("Language 'fr' was not found")));

        var result = await _updater.Execute(updaterInfo);

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error =>
            {
                error.Exception.Case.ShouldBeOfType<NotFoundException>();
                _updaterRepository.DidNotReceive().UpdateLanguage(Arg.Any<LanguageToBeUpdated>());
            }
        );
    }
}
