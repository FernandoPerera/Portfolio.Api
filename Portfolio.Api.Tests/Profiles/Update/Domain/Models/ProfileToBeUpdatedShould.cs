using LanguageExt;
using Portfolio.Api.Profiles.Update.Domain.Models;
using Shouldly;

namespace Portfolio.Api.Tests.Profiles.Update.Domain.Models;

public class ProfileToBeUpdatedShould
{
    [Fact]
    public void BeCreatedWithAllTheFields()
    {
        const string givenName = "Fernando";
        const string givenEnterprise = "Lean Mind";
        const string givenJobRole = "Beer software architect";
        const string givenPresentation = "Mucho gusto :)";
        const string givenLanguageCode = "es";

        ProfileToBeUpdated.Create(givenName, givenEnterprise, givenJobRole, givenPresentation, givenLanguageCode)
            .Match(
                profile =>
                {
                    profile.GetName().ShouldBe(Option<string>.Some(givenName));
                    profile.GetEnterprise().ShouldBe(Option<string>.Some(givenEnterprise));
                    profile.GetRole().ShouldBe(Option<string>.Some(givenJobRole));
                    profile.GetPresentation().ShouldBe(Option<string>.Some(givenPresentation));
                    profile.GetLanguageCode().ShouldBe(givenLanguageCode);
                },
                error => error.ShouldBeNull()
            );
    }

    [Fact]
    public void BeCreatedWithOnlyTheProvidedFields()
    {
        const string givenJobRole = "Beer software architect";
        const string givenLanguageCode = "es";

        ProfileToBeUpdated.Create(null, null, givenJobRole, null, givenLanguageCode)
            .Match(
                profile =>
                {
                    profile.GetName().ShouldBe(Option<string>.None);
                    profile.GetEnterprise().ShouldBe(Option<string>.None);
                    profile.GetRole().ShouldBe(Option<string>.Some(givenJobRole));
                    profile.GetPresentation().ShouldBe(Option<string>.None);
                },
                error => error.ShouldBeNull()
            );
    }

    [Fact]
    public void BeCreatedWithoutFieldsToUpdate()
    {
        ProfileToBeUpdated.Create(null, null, null, null, "es")
            .Match(
                profile =>
                {
                    profile.GetName().ShouldBe(Option<string>.None);
                    profile.GetEnterprise().ShouldBe(Option<string>.None);
                    profile.GetRole().ShouldBe(Option<string>.None);
                    profile.GetPresentation().ShouldBe(Option<string>.None);
                },
                error => error.ShouldBeNull()
            );
    }
}
