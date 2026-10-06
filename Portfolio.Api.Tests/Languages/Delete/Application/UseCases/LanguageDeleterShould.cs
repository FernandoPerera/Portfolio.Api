using LanguageExt;
using LanguageExt.Common;
using NSubstitute;
using Portfolio.Api.Languages.Delete.Application.Dtos;
using Portfolio.Api.Languages.Delete.Application.UseCases;
using Portfolio.Api.Languages.Delete.Domain.Ports;
using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Profiles.Shared.Domain.Ports;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Tests.Shared.Extensions;
using Shouldly;

namespace Portfolio.Api.Tests.Languages.Delete.Application.UseCases;

public class LanguageDeleterShould
{
    private readonly ILanguageExistenceRepository _languageExistenceRepository = Substitute.For<ILanguageExistenceRepository>();
    private readonly ILanguageUsageRepository _languageUsageRepository = Substitute.For<ILanguageUsageRepository>();
    private readonly ILanguageDeleterRepository _deleterRepository = Substitute.For<ILanguageDeleterRepository>();
    private readonly LanguageDeleter _deleter;

    public LanguageDeleterShould()
    {
        _deleter = new LanguageDeleter(_languageExistenceRepository, _languageUsageRepository, _deleterRepository);
        _languageExistenceRepository.Exists(Arg.Any<LanguageCode>()).Returns(Unit.Default);
        _languageUsageRepository.EnsureIsNotInUse(Arg.Any<LanguageCode>()).Returns(Unit.Default);
        _deleterRepository.DeleteLanguage(Arg.Any<LanguageCode>()).Returns(Unit.Default);
    }

    [Fact]
    public async Task DeleteLanguage()
    {
        const string givenLanguageCode = "es";

        var result = await _deleter.Execute(new LanguageDeleterInfo(givenLanguageCode));

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                _deleterRepository.Received(1).DeleteLanguage(Arg.Is<LanguageCode>(code => code.ToString() == givenLanguageCode));
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task FailWhenLanguageCodeIsInvalid()
    {
        var result = await _deleter.Execute(new LanguageDeleterInfo("esp"));

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error =>
            {
                error.Exception.Case.ShouldBeOfType<ValidationException>();
                _deleterRepository.DidNotReceive().DeleteLanguage(Arg.Any<LanguageCode>());
            }
        );
    }

    [Fact]
    public async Task FailWhenLanguageDoesNotExist()
    {
        _languageExistenceRepository.Exists(Arg.Any<LanguageCode>())
            .Returns(Error.New(new NotFoundException("Language 'fr' was not found")));

        var result = await _deleter.Execute(new LanguageDeleterInfo("fr"));

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error =>
            {
                error.Exception.Case.ShouldBeOfType<NotFoundException>();
                _deleterRepository.DidNotReceive().DeleteLanguage(Arg.Any<LanguageCode>());
            }
        );
    }

    [Fact]
    public async Task FailWhenLanguageIsInUse()
    {
        _languageUsageRepository.EnsureIsNotInUse(Arg.Any<LanguageCode>())
            .Returns(Error.New(new ConflictException("Language 'es' is used by existing translations")));

        var result = await _deleter.Execute(new LanguageDeleterInfo("es"));

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error =>
            {
                error.Exception.Case.ShouldBeOfType<ConflictException>();
                _deleterRepository.DidNotReceive().DeleteLanguage(Arg.Any<LanguageCode>());
            }
        );
    }
}
