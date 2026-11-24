using Portfolio.Api.Tests.WaysOfWorking.Shared.Domain.Builders;
using Portfolio.Api.WaysOfWorking.Find.Domain.Models;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;

namespace Portfolio.Api.Tests.WaysOfWorking.Find.Domain.Builders;

public static class WayOfWorkingFoundBuilder
{
    public static WayOfWorkingFound Build(
        Guid? id = null,
        int position = 1,
        string illustration = "plain-language",
        List<WayTranslation>? translations = null
    )
    {
        return WayOfWorkingFound.CreateUnsafe(
                id ?? Guid.NewGuid(),
                position,
                illustration,
                translations ?? [WayTranslationBuilder.Build()]
            )
            .IfLeft(error => throw new InvalidOperationException($"Error building {nameof(WayOfWorkingFound)} in tests: {error}"));
    }
}
