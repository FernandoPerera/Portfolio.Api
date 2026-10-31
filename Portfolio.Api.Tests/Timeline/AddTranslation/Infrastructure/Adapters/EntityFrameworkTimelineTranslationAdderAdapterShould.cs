using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Shared.Extensions;
using Portfolio.Api.Tests.Timeline.AddTranslation.Domain.Builders;
using Portfolio.Api.Tests.Timeline.Create.Infrastructure.Builders;
using Portfolio.Api.Timeline.AddTranslation.Infrastructure.Adapters;
using Portfolio.Api.Timeline.Create.Infrastructure.Entities;
using Shouldly;

namespace Portfolio.Api.Tests.Timeline.AddTranslation.Infrastructure.Adapters;

public class EntityFrameworkTimelineTranslationAdderAdapterShould
{
    private readonly AppDbContext _dbContext;
    private readonly EntityFrameworkTimelineTranslationAdderAdapter _adder;

    public EntityFrameworkTimelineTranslationAdderAdapterShould()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);
        _adder = new EntityFrameworkTimelineTranslationAdderAdapter(_dbContext);
    }

    [Fact]
    public async Task AddTheTranslation()
    {
        const string givenLanguage = "en";
        const string givenTitle = "Frontend Lead";
        const string givenDescription = "Technical leadership.";
        var entryId = Guid.NewGuid();
        var existing = TimelineEntryTranslationEntityBuilder.Build(entryId, "es");
        await _dbContext.TimelineEntries.AddAsync(TimelineEntryEntityBuilder.Build(entryId, translations: [existing]));
        await _dbContext.SaveChangesAsync();
        var translation = TimelineTranslationToBeAddedBuilder.Build(entryId, givenLanguage, givenTitle, givenDescription);

        var result = await _adder.AddTranslation(translation);

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                AssertStoredTranslations(
                    TimelineEntryTranslationEntityBuilder.Build(entryId, givenLanguage, givenTitle, givenDescription),
                    existing
                );
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task FailWhenErrorOccurs()
    {
        var translation = TimelineTranslationToBeAddedBuilder.Build();
        await _dbContext.DisposeAsync();

        var result = await _adder.AddTranslation(translation);

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error => error.ShouldBeOfType<ServiceUnavailableError>()
        );
    }

    private void AssertStoredTranslations(params TimelineEntryTranslationEntity[] expected) =>
        ToComparable(_dbContext.TimelineEntryTranslations).ShouldBeEquivalentTo(ToComparable(expected));

    private static List<object> ToComparable(IEnumerable<TimelineEntryTranslationEntity> translations) =>
        translations
            .OrderBy(translation => translation.LanguageCode)
            .Select(translation => (object)new
            {
                translation.EntryId,
                translation.LanguageCode,
                translation.Title,
                translation.Description
            })
            .ToList();
}
