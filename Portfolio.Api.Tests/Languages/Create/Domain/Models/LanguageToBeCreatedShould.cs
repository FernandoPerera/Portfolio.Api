using Portfolio.Api.Languages.Create.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;
using Shouldly;

namespace Portfolio.Api.Tests.Languages.Create.Domain.Models;

public class LanguageToBeCreatedShould
{
    [Fact]
    public void BeCreated()
    {
        LanguageToBeCreated.Create("ES", "Español")
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
        LanguageToBeCreated.Create("esp", "Español")
            .Match(
                language => language.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("Language code must have 2 letters")
            );
    }

    [Fact]
    public void RejectLanguageWhenNameIsEmpty()
    {
        LanguageToBeCreated.Create("es", " ")
            .Match(
                language => language.ShouldBeNull(),
                error => error.ShouldBeOfType<ValidationError>()
                    .Message.ShouldBe("Language name cannot be empty")
            );
    }
}
