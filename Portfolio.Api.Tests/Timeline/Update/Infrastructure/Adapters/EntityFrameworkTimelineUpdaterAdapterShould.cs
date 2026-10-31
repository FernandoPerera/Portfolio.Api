using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Shared.Extensions;
using Portfolio.Api.Tests.Timeline.Create.Infrastructure.Builders;
using Portfolio.Api.Tests.Timeline.Find.Domain.Builders;
using Portfolio.Api.Tests.Timeline.Shared.Domain.Builders;
using Portfolio.Api.Timeline.Find.Domain.Models;
using Portfolio.Api.Timeline.Update.Domain.Models;
using Portfolio.Api.Timeline.Update.Infrastructure.Adapters;
using Shouldly;

namespace Portfolio.Api.Tests.Timeline.Update.Infrastructure.Adapters;

public class EntityFrameworkTimelineUpdaterAdapterShould
{
    private readonly AppDbContext _dbContext;
    private readonly EntityFrameworkTimelineUpdaterAdapter _updater;

    public EntityFrameworkTimelineUpdaterAdapterShould()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);
        _updater = new EntityFrameworkTimelineUpdaterAdapter(_dbContext);
    }

    [Fact]
    public async Task UpdateOnlyTheGivenFields()
    {
        var entryId = Guid.NewGuid();
        var original = TimelineEntryEntityBuilder.Build(entryId, endDate: new DateOnly(2023, 6, 1));
        await _dbContext.TimelineEntries.AddAsync(original);
        await _dbContext.SaveChangesAsync();
        var expected = TimelineEntryEntityBuilder.Build(
            entryId,
            "Lean Mind Labs",
            endDate: new DateOnly(2023, 6, 1),
            translations: [TimelineEntryTranslationEntityBuilder.Build(entryId)]
        );
        var current = TimelineEntryFoundBuilder.Build(
            entryId,
            endDate: new DateOnly(2023, 6, 1),
            technologies: ["React", "TypeScript"]
        );
        var update = ToBeUpdated(current, organization: "Lean Mind Labs");

        var result = await _updater.UpdateEntry(update);

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                _dbContext.TimelineEntries.Include(e => e.Translations).Single().ShouldBeEquivalentTo(expected);
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task UpdateAllTheGivenFields()
    {
        var entryId = Guid.NewGuid();
        await _dbContext.TimelineEntries.AddAsync(TimelineEntryEntityBuilder.Build(entryId));
        await _dbContext.SaveChangesAsync();
        var expected = TimelineEntryEntityBuilder.Build(
            entryId,
            "Escuela",
            "education",
            new DateOnly(2021, 9, 1),
            new DateOnly(2023, 6, 1),
            ["Angular"],
            [TimelineEntryTranslationEntityBuilder.Build(entryId, "es", "Ciclo formativo", "Desarrollo web.")]
        );
        var update = ToBeUpdated(
            TimelineEntryFoundBuilder.Build(entryId),
            "Escuela",
            "education",
            "2021-09",
            "2023-06",
            ["Angular"],
            "Ciclo formativo",
            "Desarrollo web."
        );

        var result = await _updater.UpdateEntry(update);

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                _dbContext.TimelineEntries.Include(e => e.Translations).Single().ShouldBeEquivalentTo(expected);
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task UpdateOnlyTheTranslationOfTheGivenLanguage()
    {
        const string givenLanguage = "es";
        const string untouchedLanguage = "en";
        var entryId = Guid.NewGuid();
        var spanish = TimelineEntryTranslationEntityBuilder.Build(entryId, givenLanguage);
        var english = TimelineEntryTranslationEntityBuilder.Build(entryId, untouchedLanguage, "Frontend Lead", "Technical leadership.");
        await _dbContext.TimelineEntries.AddAsync(TimelineEntryEntityBuilder.Build(entryId, translations: [spanish, english]));
        await _dbContext.SaveChangesAsync();
        var current = TimelineEntryFoundBuilder.Build(
            entryId,
            technologies: ["React", "TypeScript"],
            translations:
            [
                TimelineTranslationBuilder.Build(givenLanguage),
                TimelineTranslationBuilder.Build(untouchedLanguage, "Frontend Lead", "Technical leadership.")
            ]
        );
        var update = ToBeUpdated(current, title: "Nuevo título", description: "Nueva descripción.", languageCode: givenLanguage);

        var result = await _updater.UpdateEntry(update);

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                _dbContext.TimelineEntryTranslations
                    .OrderBy(translation => translation.LanguageCode)
                    .Select(translation => new { translation.LanguageCode, translation.Title, translation.Description })
                    .ToList()
                    .ShouldBeEquivalentTo(new[]
                    {
                        new { LanguageCode = untouchedLanguage, Title = "Frontend Lead", Description = "Technical leadership." },
                        new { LanguageCode = givenLanguage, Title = "Nuevo título", Description = "Nueva descripción." }
                    }.ToList());
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task FailWhenTheEntryDoesNotExist()
    {
        var update = ToBeUpdated(TimelineEntryFoundBuilder.Build(), organization: "Lean Mind Labs");

        var result = await _updater.UpdateEntry(update);

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error => error.ShouldBeOfType<NotFoundError>()
        );
    }

    [Fact]
    public async Task FailWhenTheTranslationDoesNotExist()
    {
        var entryId = Guid.NewGuid();
        await _dbContext.TimelineEntries.AddAsync(TimelineEntryEntityBuilder.Build(entryId));
        await _dbContext.SaveChangesAsync();
        var current = TimelineEntryFoundBuilder.Build(
            entryId,
            translations: [TimelineTranslationBuilder.Build("fr")]
        );
        var update = ToBeUpdated(current, languageCode: "fr");

        var result = await _updater.UpdateEntry(update);

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error => error.ShouldBeOfType<NotFoundError>()
                .Message.ShouldBe("Timeline translation for language 'fr' was not found")
        );
    }

    [Fact]
    public async Task FailWhenErrorOccurs()
    {
        var update = ToBeUpdated(TimelineEntryFoundBuilder.Build(), organization: "Lean Mind Labs");
        await _dbContext.DisposeAsync();

        var result = await _updater.UpdateEntry(update);

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error => error.ShouldBeOfType<ServiceUnavailableError>()
        );
    }

    private static TimelineEntryToBeUpdated ToBeUpdated(
        TimelineEntryFound current,
        string? organization = null,
        string? category = null,
        string? startDate = null,
        string? endDate = null,
        List<string>? technologies = null,
        string? title = null,
        string? description = null,
        string languageCode = "es"
    ) =>
        TimelineEntryToBeUpdated.Create(
                current,
                organization,
                category,
                startDate,
                endDate,
                technologies,
                title,
                description,
                languageCode
            )
            .IfLeft(error => throw new Exception("Error building the update in tests, error: " + error.Message));
}
