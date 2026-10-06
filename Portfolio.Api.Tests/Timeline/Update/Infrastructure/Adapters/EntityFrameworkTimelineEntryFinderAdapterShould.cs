using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Timeline.Create.Infrastructure.Builders;
using Portfolio.Api.Timeline.Update.Infrastructure.Adapters;
using Shouldly;

namespace Portfolio.Api.Tests.Timeline.Update.Infrastructure.Adapters;

public class EntityFrameworkTimelineEntryFinderAdapterShould
{
    private readonly AppDbContext _dbContext;
    private readonly EntityFrameworkTimelineEntryFinderAdapter _finder;

    public EntityFrameworkTimelineEntryFinderAdapterShould()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);
        _finder = new EntityFrameworkTimelineEntryFinderAdapter(_dbContext);
    }

    [Fact]
    public async Task FindEntryWithItsTranslations()
    {
        var entryId = Guid.NewGuid();
        var spanish = TimelineEntryTranslationEntityBuilder.Build(entryId, "es");
        var english = TimelineEntryTranslationEntityBuilder.Build(entryId, "en");
        var entity = TimelineEntryEntityBuilder.Build(
            entryId,
            endDate: new DateOnly(2023, 6, 1),
            translations: [spanish, english]
        );
        await _dbContext.TimelineEntries.AddAsync(entity);
        await _dbContext.TimelineEntries.AddAsync(TimelineEntryEntityBuilder.Build(Guid.NewGuid()));
        await _dbContext.SaveChangesAsync();

        var result = await _finder.FindEntry(entryId);

        result.Match(
            entry =>
            {
                entry.GetId().ShouldBe(entryId);
                entry.GetOrganization().ShouldBe(entity.Organization);
                entry.GetCategory().ShouldBe(entity.Category);
                entry.GetStartDate().ShouldBe(entity.StartDate.ToString("yyyy-MM"));
                entry.GetEndDate().ShouldBe("2023-06");
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
    public async Task FailWhenTheEntryDoesNotExist()
    {
        await _dbContext.TimelineEntries.AddAsync(TimelineEntryEntityBuilder.Build(Guid.NewGuid()));
        await _dbContext.SaveChangesAsync();

        var result = await _finder.FindEntry(Guid.NewGuid());

        result.Match(
            entry => entry.ShouldBeNull(),
            error => error.Exception.Case.ShouldBeOfType<NotFoundException>()
        );
    }

    [Fact]
    public async Task FailWhenUnexpectedErrorOccurs()
    {
        await _dbContext.DisposeAsync();

        var result = await _finder.FindEntry(Guid.NewGuid());

        result.Match(
            entry => entry.ShouldBeNull(),
            error => error.Exception.Case.ShouldBeOfType<ServiceUnavailableException>()
        );
    }
}
