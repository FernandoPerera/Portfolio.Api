using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Shared.Extensions;
using Portfolio.Api.Tests.Shared.Infrastructure;
using Portfolio.Api.Tests.WaysOfWorking.Create.Infrastructure.Builders;
using Portfolio.Api.Tests.WaysOfWorking.UpdateTranslation.Domain.Builders;
using Portfolio.Api.WaysOfWorking.Shared.Infrastructure.Entities;
using Portfolio.Api.WaysOfWorking.UpdateTranslation.Infrastructure.Adapters;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.UpdateTranslation.Infrastructure.Adapters;

public class EntityFrameworkWayTranslationUpdaterAdapterShould
{
    private readonly AppDbContext _dbContext;
    private readonly EntityFrameworkWayTranslationUpdaterAdapter _updater;

    public EntityFrameworkWayTranslationUpdaterAdapterShould()
    {
        var options = PostgresTestDatabase.CreateOptions();

        _dbContext = new AppDbContext(options);

        _dbContext.SeedLanguages("es", "en");
        _updater = new EntityFrameworkWayTranslationUpdaterAdapter(_dbContext);
    }

    [Fact]
    public async Task UpdateOnlyTheTranslationOfTheGivenLanguage()
    {
        var wayId = Guid.NewGuid();
        var english = WayOfWorkingTranslationEntityBuilder.Build(wayId, "en", "Clear");
        await _dbContext.WaysOfWorking.AddAsync(WayOfWorkingEntityBuilder.Build(
            wayId,
            translations: [WayOfWorkingTranslationEntityBuilder.Build(wayId, "es"), english]
        ));
        await _dbContext.SaveChangesAsync();
        var translation = WayTranslationToBeUpdatedBuilder.Build(
            wayId,
            "es",
            "Claro y simple",
            title: "Te lo explico fácil",
            points: ["Una charla", "Un resumen"]
        );

        var result = await _updater.UpdateTranslation(translation);

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                var stored = _dbContext.WayOfWorkingTranslations.AsNoTracking().OrderBy(t => t.LanguageCode).ToList();
                stored.Select(ToComparable).ToList().ShouldBeEquivalentTo(new List<object>
                {
                    ToComparable(english),
                    ToComparable(WayOfWorkingTranslationEntityBuilder.Build(
                        wayId,
                        "es",
                        "Claro y simple",
                        "Sin jerga",
                        "Te lo explico fácil",
                        "Antes de escribir una línea hablamos de lo que necesitas con palabras normales.",
                        ["Una charla", "Un resumen"]
                    ))
                });
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task FailWhenErrorOccurs()
    {
        var translation = WayTranslationToBeUpdatedBuilder.Build();
        await _dbContext.DisposeAsync();

        var result = await _updater.UpdateTranslation(translation);

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error => error.ShouldBeOfType<ServiceUnavailableError>()
        );
    }

    private static object ToComparable(WayOfWorkingTranslationEntity translation) => new
    {
        translation.WayId,
        translation.LanguageCode,
        translation.Label,
        translation.Caption,
        translation.Title,
        translation.Description,
        translation.Points
    };
}
