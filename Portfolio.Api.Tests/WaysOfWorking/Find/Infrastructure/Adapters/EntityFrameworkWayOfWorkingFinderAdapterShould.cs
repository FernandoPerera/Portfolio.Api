using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Shared.Infrastructure;
using Portfolio.Api.Tests.WaysOfWorking.Create.Infrastructure.Builders;
using Portfolio.Api.WaysOfWorking.Find.Infrastructure.Adapters;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.Find.Infrastructure.Adapters;

public class EntityFrameworkWayOfWorkingFinderAdapterShould
{
    private readonly AppDbContext _dbContext;
    private readonly EntityFrameworkWayOfWorkingFinderAdapter _adapter;

    public EntityFrameworkWayOfWorkingFinderAdapterShould()
    {
        var options = PostgresTestDatabase.CreateOptions();

        _dbContext = new AppDbContext(options);

        _dbContext.SeedLanguages("es", "en");
        _adapter = new EntityFrameworkWayOfWorkingFinderAdapter(_dbContext);
    }

    [Fact]
    public async Task FindWayOfWorkingWithItsTranslations()
    {
        var wayId = Guid.NewGuid();
        var english = WayOfWorkingTranslationEntityBuilder.Build(wayId, "en", "Clear", points: ["A short chat"]);
        var spanish = WayOfWorkingTranslationEntityBuilder.Build(wayId, "es", points: ["Una charla", "Un resumen"]);
        var entity = WayOfWorkingEntityBuilder.Build(wayId, 2, "tested", [spanish, english]);
        await _dbContext.WaysOfWorking.AddAsync(entity);
        await _dbContext.SaveChangesAsync();

        var result = await _adapter.FindWays();

        result.Match(
            ways =>
            {
                var way = ways.ShouldHaveSingleItem();
                way.GetId().ShouldBe(wayId);
                way.GetPosition().ShouldBe(2);
                way.GetIllustration().ShouldBe("tested");
                way.GetTranslations()
                    .Select(translation => (
                        translation.GetLanguageCode(),
                        translation.GetLabel(),
                        translation.GetCaption(),
                        translation.GetTitle(),
                        translation.GetDescription(),
                        string.Join(" | ", translation.GetPoints())
                    ))
                    .ShouldBe(
                    [
                        (english.LanguageCode, english.Label, english.Caption, english.Title, english.Description, string.Join(" | ", english.Points)),
                        (spanish.LanguageCode, spanish.Label, spanish.Caption, spanish.Title, spanish.Description, string.Join(" | ", spanish.Points))
                    ]);
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task FindWaysOfWorkingInTheirPositionOrder()
    {
        await _dbContext.WaysOfWorking.AddRangeAsync(
            WayOfWorkingEntityBuilder.Build(Guid.NewGuid(), 3, "third"),
            WayOfWorkingEntityBuilder.Build(Guid.NewGuid(), 1, "first"),
            WayOfWorkingEntityBuilder.Build(Guid.NewGuid(), 2, "second")
        );
        await _dbContext.SaveChangesAsync();

        var result = await _adapter.FindWays();

        result.Match(
            ways => ways.Select(way => way.GetIllustration()).ShouldBe(["first", "second", "third"]),
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task FindNoWaysOfWorkingWhenThereAreNone()
    {
        var result = await _adapter.FindWays();

        result.Match(
            ways => ways.ShouldBeEmpty(),
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task FailWhenUnexpectedErrorOccurs()
    {
        await _dbContext.DisposeAsync();

        var result = await _adapter.FindWays();

        result.Match(
            ways => ways.ShouldBeNull(),
            error => error.ShouldBeOfType<ServiceUnavailableError>()
        );
    }
}
