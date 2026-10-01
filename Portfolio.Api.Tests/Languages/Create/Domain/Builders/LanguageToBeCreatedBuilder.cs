using Portfolio.Api.Languages.Create.Domain.Models;

namespace Portfolio.Api.Tests.Languages.Create.Domain.Builders;

public abstract class LanguageToBeCreatedBuilder
{
    public static LanguageToBeCreated Build(
        string languageCode = "es",
        string name = "Español"
    )
    {
        return LanguageToBeCreated.Create(languageCode, name)
            .IfLeft(error => throw new Exception("Error in LanguageToBeCreatedBuilder, error: " + error.Message));
    }
}
