using Portfolio.Api.Languages.Update.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;
using Shouldly;

namespace Portfolio.Api.Tests.Languages.Update.Domain.Models;

public class LanguageToBeUpdatedShould
{
    [Fact]
    public void BeCreated()
    {
        LanguageToBeUpdated.Create("ES", "Español")
            .Match(
                language =>
                {
                    language.GetLanguageCode().ShouldBe("es");
                    language.GetName().ShouldBe("Español");
                },
                error => error.ShouldBeNull()
            );
    }

    [Fact]
    public void RejectLanguageWhenCodeIsInvalid()
    {
        LanguageToBeUpdated.Create("esp", "Español")
            .Match(
                language => language.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("Language code must have 2 letters")
            );
    }

    [Fact]
    public void RejectLanguageWhenNameIsEmpty()
    {
        LanguageToBeUpdated.Create("es", " ")
            .Match(
                language => language.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("Language name cannot be empty")
            );
    }
}
