using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Timeline.Create.Infrastructure.Builders;
using Portfolio.Api.Timeline.Find.Infrastructure.Adapters;
using Shouldly;

namespace Portfolio.Api.Tests.Timeline.Find.Infrastructure.Adapters;

public class EntityFrameworkTimelineFinderAdapterShould
{
    private readonly AppDbContext _dbContext;
    private readonly EntityFrameworkTimelineFinderAdapter _adapter;

    public EntityFrameworkTimelineFinderAdapterShould()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);
        _adapter = new EntityFrameworkTimelineFinderAdapter(_dbContext);
    }

    [Fact]
    public async Task FindEntryWithItsTranslations()
    {
        var entryId = Guid.NewGuid();
        var endDate = new DateOnly(2023, 6, 1);
        var spanish = TimelineEntryTranslationEntityBuilder.Build(entryId, "es");
        var english = TimelineEntryTranslationEntityBuilder.Build(entryId, "en");
        var entity = TimelineEntryEntityBuilder.Build(
            entryId,
            endDate: endDate,
            translations: [spanish, english]
        );
        await _dbContext.TimelineEntries.AddAsync(entity);
        await _dbContext.SaveChangesAsync();

        var result = await _adapter.FindEntries();

        result.Match(
            entries =>
            {
                var entry = entries.ShouldHaveSingleItem();
                entry.GetId().ShouldBe(entryId);
                entry.GetOrganization().ShouldBe(entity.Organization);
                entry.GetCategory().ShouldBe(entity.Category);
                entry.GetStartDate().ShouldBe(entity.StartDate.ToString("yyyy-MM"));
                entry.GetEndDate().ShouldBe(endDate.ToString("yyyy-MM"));
                entry.GetTechnologies().ShouldBe(entity.Technologies);
                entry.GetTranslations()
                    .Select(translation => (
                        translation.GetLanguageCode(),
                        translation.GetTitle(),
                        translation.GetDescription()
                    ))
                    .ShouldBe(
                    [
                        (spanish.LanguageCode, spanish.Title, spanish.Description),
                        (english.LanguageCode, english.Title, english.Description)
                    ]);
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task FindOngoingEntryWithoutEndDate()
    {
        await _dbContext.TimelineEntries.AddAsync(TimelineEntryEntityBuilder.Build(Guid.NewGuid(), endDate: null));
        await _dbContext.SaveChangesAsync();

        var result = await _adapter.FindEntries();

        result.Match(
            entries => entries.ShouldHaveSingleItem().GetEndDate().ShouldBeNull(),
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task FindEntriesFromNewestToOldest()
    {
        await _dbContext.TimelineEntries.AddRangeAsync(
            TimelineEntryEntityBuilder.Build(Guid.NewGuid(), "Oldest", startDate: new DateOnly(2021, 1, 1)),
            TimelineEntryEntityBuilder.Build(Guid.NewGuid(), "Newest", startDate: new DateOnly(2024, 3, 1)),
            TimelineEntryEntityBuilder.Build(Guid.NewGuid(), "Middle", startDate: new DateOnly(2022, 9, 1))
        );
        await _dbContext.SaveChangesAsync();

        var result = await _adapter.FindEntries();

        result.Match(
            entries => entries.Select(entry => entry.GetOrganization()).ShouldBe(["Newest", "Middle", "Oldest"]),
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task FindNoEntriesWhenTimelineIsEmpty()
    {
        var result = await _adapter.FindEntries();

        result.Match(
            entries => entries.ShouldBeEmpty(),
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task FailWhenUnexpectedErrorOccurs()
    {
        await _dbContext.DisposeAsync();

        var result = await _adapter.FindEntries();

        result.Match(
            entries => entries.ShouldBeNull(),
            error => error.Exception.Case.ShouldBeOfType<ServiceUnavailableException>()
        );
    }
}
