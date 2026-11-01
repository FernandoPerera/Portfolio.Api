using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Shared.Extensions;
using Portfolio.Api.Tests.Timeline.Create.Infrastructure.Builders;
using Portfolio.Api.Tests.Timeline.Shared.Domain.Builders;
using Portfolio.Api.Tests.Shared.Infrastructure;
using Portfolio.Api.Timeline.Delete.Infrastructure.Adapters;
using Shouldly;

namespace Portfolio.Api.Tests.Timeline.Delete.Infrastructure.Adapters;

public class EntityFrameworkTimelineDeleterAdapterShould
{
    private readonly AppDbContext _dbContext;
    private readonly EntityFrameworkTimelineDeleterAdapter _deleter;

    public EntityFrameworkTimelineDeleterAdapterShould()
    {
        var options = PostgresTestDatabase.CreateOptions();

        _dbContext = new AppDbContext(options);

        _dbContext.SeedLanguages("es", "en");
        _deleter = new EntityFrameworkTimelineDeleterAdapter(_dbContext);
    }

    [Fact]
    public async Task DeleteTheEntryAndItsTranslations()
    {
        var deletedId = Guid.NewGuid();
        var untouchedId = Guid.NewGuid();
        await _dbContext.TimelineEntries.AddRangeAsync(
            TimelineEntryEntityBuilder.Build(
                deletedId,
                translations:
                [
                    TimelineEntryTranslationEntityBuilder.Build(deletedId, "es"),
                    TimelineEntryTranslationEntityBuilder.Build(deletedId, "en")
                ]
            ),
            TimelineEntryEntityBuilder.Build(untouchedId)
        );
        await _dbContext.SaveChangesAsync();

        var result = await _deleter.DeleteEntry(TimelineIdBuilder.Build(deletedId));

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                _dbContext.TimelineEntries.Select(entry => entry.Id).ToList().ShouldBe([untouchedId]);
                _dbContext.TimelineEntryTranslations.Select(translation => translation.EntryId).ToList()
                    .ShouldBe([untouchedId]);
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task FailWhenTheEntryDoesNotExist()
    {
        var result = await _deleter.DeleteEntry(TimelineIdBuilder.Build());

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error => error.ShouldBeOfType<NotFoundError>()
        );
    }

    [Fact]
    public async Task FailWhenErrorOccurs()
    {
        var id = TimelineIdBuilder.Build();
        await _dbContext.DisposeAsync();

        var result = await _deleter.DeleteEntry(id);

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error => error.ShouldBeOfType<ServiceUnavailableError>()
        );
    }
}
