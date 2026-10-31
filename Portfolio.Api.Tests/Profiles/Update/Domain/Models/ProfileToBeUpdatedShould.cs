using Portfolio.Api.Profiles.Update.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Tests.Profiles.Find.Domain.Builders;
using Shouldly;

namespace Portfolio.Api.Tests.Profiles.Update.Domain.Models;

public class ProfileToBeUpdatedShould
{
    [Fact]
    public void BeCreatedWithAllTheGivenFields()
    {
        const string givenName = "Fernando";
        const string givenEnterprise = "Lean Mind";
        const string givenJobRole = "Beer software architect";
        const string givenPresentation = "Mucho gusto :)";
        const string givenLanguageCode = "es";
        var current = ProfileFoundBuilder.Build(
            "Old name",
            "Old enterprise",
            [ProfileTranslationFoundBuilder.Build(givenLanguageCode, "Old presentation", "Old role")]
        );

        ProfileToBeUpdated.Create(current, givenName, givenEnterprise, givenJobRole, givenPresentation, givenLanguageCode)
            .Match(
                profile =>
                {
                    profile.GetName().ShouldBe(givenName);
                    profile.GetEnterprise().ShouldBe(givenEnterprise);
                    profile.GetRole().ShouldBe(givenJobRole);
                    profile.GetPresentation().ShouldBe(givenPresentation);
                    profile.GetLanguageCode().ShouldBe(givenLanguageCode);
                },
                error => error.ShouldBeNull()
            );
    }

    [Fact]
    public void KeepTheCurrentValuesThatWereNotGiven()
    {
        const string givenJobRole = "Beer software architect";
        const string givenLanguageCode = "es";
        var translation = ProfileTranslationFoundBuilder.Build(givenLanguageCode, "Current presentation", "Current role");
        var current = ProfileFoundBuilder.Build("Current name", "Current enterprise", [translation]);

        ProfileToBeUpdated.Create(current, null, null, givenJobRole, null, givenLanguageCode)
            .Match(
                profile =>
                {
                    profile.GetName().ShouldBe(current.GetName());
                    profile.GetEnterprise().ShouldBe(current.GetEnterprise());
                    profile.GetRole().ShouldBe(givenJobRole);
                    profile.GetPresentation().ShouldBe(translation.GetPresentation());
                },
                error => error.ShouldBeNull()
            );
    }

    [Fact]
    public void KeepTheCurrentProfileWithoutFieldsToUpdate()
    {
        var translation = ProfileTranslationFoundBuilder.Build();
        var current = ProfileFoundBuilder.Build(translations: [translation]);

        ProfileToBeUpdated.Create(current, null, null, null, null, translation.GetLanguageCode())
            .Match(
                profile =>
                {
                    profile.GetName().ShouldBe(current.GetName());
                    profile.GetEnterprise().ShouldBe(current.GetEnterprise());
                    profile.GetRole().ShouldBe(translation.GetRole());
                    profile.GetPresentation().ShouldBe(translation.GetPresentation());
                },
                error => error.ShouldBeNull()
            );
    }

    [Fact]
    public void RejectUpdateWhenTheLanguageIsNotInTheProfile()
    {
        var current = ProfileFoundBuilder.Build(translations: [ProfileTranslationFoundBuilder.Build("es")]);

        ProfileToBeUpdated.Create(current, null, null, null, null, "fr")
            .Match(
                profile => profile.ShouldBeNull(),
                error => error.ShouldBeOfType<NotFoundError>()
                    .Message.ShouldBe("Profile translation for language 'fr' was not found")
            );
    }
}
