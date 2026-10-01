using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Domain.Ports;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Shared.Extensions;
using Portfolio.Api.Tests.Timeline.Create.Domain.Builders;
using Portfolio.Api.Tests.Timeline.Create.Infrastructure.Builders;
using Portfolio.Api.Timeline.Create.Infrastructure.Adapters;
using Shouldly;

namespace Portfolio.Api.Tests.Timeline.Create.Infrastructure.Adapters;

public class EntityFrameworkTimelineCreatorAdapterShould
{
    private readonly AppDbContext _appDbContext;
    private readonly IGeneratorIdentifier _idGenerator = Substitute.For<IGeneratorIdentifier>();
    private readonly EntityFrameworkTimelineCreatorAdapter _creator;

    public EntityFrameworkTimelineCreatorAdapterShould()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _appDbContext = new AppDbContext(options);
        _creator = new EntityFrameworkTimelineCreatorAdapter(_appDbContext, _idGenerator);
    }

    public static TheoryData<string?, DateOnly?> EndDates => new()
    {
        { "2023-06", new DateOnly(2023, 6, 1) },
        { null, null }
    };

    [Theory]
    [MemberData(nameof(EndDates))]
    public async Task CreateTimelineEntry(string? givenEndDate, DateOnly? expectedEndDate)
    {
        var entryId = Guid.NewGuid();
        _idGenerator.Generate().Returns(entryId);
        var spanish = TimelineTranslationBuilder.Build("es", "Líder Frontend", "Liderazgo técnico.");
        var english = TimelineTranslationBuilder.Build("en", "Frontend Lead", "Technical leadership.");
        var entry = TimelineEntryToBeCreatedBuilder.Build(endDate: givenEndDate, translations: [spanish, english]);
        var expected = TimelineEntryEntityBuilder.Build(
            entryId,
            endDate: expectedEndDate,
            translations:
            [
                TimelineEntryTranslationEntityBuilder.Build(
                    entryId, spanish.GetLanguageCode(), spanish.GetTitle(), spanish.GetDescription()
                ),
                TimelineEntryTranslationEntityBuilder.Build(
                    entryId, english.GetLanguageCode(), english.GetTitle(), english.GetDescription()
                )
            ]
        );

        var result = await _creator.CreateEntry(entry);

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                _appDbContext.TimelineEntries.Include(e => e.Translations).Single().ShouldBeEquivalentTo(expected);
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task FailWhenErrorOccurs()
    {
        var entry = TimelineEntryToBeCreatedBuilder.Build();
        _idGenerator.Generate().Returns(Guid.NewGuid());
        await _appDbContext.DisposeAsync();

        var result = await _creator.CreateEntry(entry);

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error => error.Exception.Case.ShouldBeOfType<ServiceUnavailableException>()
        );
    }
}
