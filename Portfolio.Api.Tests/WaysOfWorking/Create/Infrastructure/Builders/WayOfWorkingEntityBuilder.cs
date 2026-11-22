using Portfolio.Api.WaysOfWorking.Shared.Infrastructure.Entities;

namespace Portfolio.Api.Tests.WaysOfWorking.Create.Infrastructure.Builders;

public static class WayOfWorkingEntityBuilder
{
    public static WayOfWorkingEntity Build(
        Guid wayId,
        int position = 1,
        string illustration = "plain-language",
        List<WayOfWorkingTranslationEntity>? translations = null
    )
    {
        var way = new WayOfWorkingEntity
        {
            Id = wayId,
            Position = position,
            Illustration = illustration,
            Translations = translations ?? [WayOfWorkingTranslationEntityBuilder.Build(wayId)]
        };

        foreach (var translation in way.Translations)
        {
            translation.Way = way;
        }

        return way;
    }
}
