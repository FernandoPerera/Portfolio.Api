using Portfolio.Api.Languages.Update.Domain.Models;

namespace Portfolio.Api.Tests.Languages.Update.Domain.Builders;

public abstract class LanguageToBeUpdatedBuilder
{
    public static LanguageToBeUpdated Build(
        string languageCode = "es",
        string name = "Spanish"
    )
    {
        return LanguageToBeUpdated.Create(languageCode, name)
            .IfLeft(error => throw new Exception("Error in LanguageToBeUpdatedBuilder, error: " + error.Message));
    }
}
