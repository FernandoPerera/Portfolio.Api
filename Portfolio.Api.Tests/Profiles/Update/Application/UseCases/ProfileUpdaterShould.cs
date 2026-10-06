using LanguageExt;
using NSubstitute;
using Portfolio.Api.Profiles.Update.Application.UseCases;
using Portfolio.Api.Profiles.Update.Domain.Models;
using Portfolio.Api.Profiles.Update.Domain.Ports;
using Portfolio.Api.Tests.Profiles.Update.Application.Builders;
using Portfolio.Api.Tests.Shared.Extensions;
using Shouldly;

namespace Portfolio.Api.Tests.Profiles.Update.Application.UseCases;

public class ProfileUpdaterShould
{
    private readonly IProfileUpdaterRepository _updaterRepository = Substitute.For<IProfileUpdaterRepository>();
    private readonly ProfileUpdater _updater;

    public ProfileUpdaterShould()
    {
        _updater = new ProfileUpdater(_updaterRepository);
    }

    [Fact]
    public async Task UpdateProfile()
    {
        const string givenJobRole = "Software Engineer";
        const string givenLanguageCode = "en";
        var updaterInfo = ProfileUpdaterInfoBuilder.Build(
            name: null,
            enterpriseName: null,
            jobRole: givenJobRole,
            presentation: null,
            languageCode: givenLanguageCode
        );
        _updaterRepository.UpdateProfile(Arg.Any<ProfileToBeUpdated>()).Returns(Unit.Default);

        var result = await _updater.Execute(updaterInfo);

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                _updaterRepository.Received(1).UpdateProfile(Arg.Is<ProfileToBeUpdated>(profile =>
                    profile.GetRole() == Option<string>.Some(givenJobRole)
                    && profile.GetName() == Option<string>.None
                    && profile.GetLanguageCode() == givenLanguageCode
                ));
            },
            error => error.ShouldBeNull()
        );
    }
}
