using Portfolio.Api.Languages.Find.Domain.Models;

namespace Portfolio.Api.Tests.Languages.Find.Domain.Builders;

public static class LanguageFoundBuilder
{
    public static LanguageFound Build(
        string languageCode = "es",
        string name = "Español"
    )
    {
        return LanguageFound.CreateUnsafe(languageCode, name)
            .IfLeft(error => throw new InvalidOperationException($"Error building {nameof(LanguageFound)} in tests: {error}"));
    }
}
