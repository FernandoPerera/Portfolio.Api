using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Shared.Infrastructure;
using Portfolio.Api.Tests.WaysOfWorking.Create.Infrastructure.Builders;
using Portfolio.Api.Tests.WaysOfWorking.Shared.Domain.Builders;
using Portfolio.Api.WaysOfWorking.Shared.Infrastructure.Adapters;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.Shared.Infrastructure.Adapters;

public class EntityFrameworkWayOfWorkingByIdFinderAdapterShould
{
    private readonly AppDbContext _dbContext;
    private readonly EntityFrameworkWayOfWorkingByIdFinderAdapter _finder;

    public EntityFrameworkWayOfWorkingByIdFinderAdapterShould()
    {
        var options = PostgresTestDatabase.CreateOptions();

        _dbContext = new AppDbContext(options);

        _dbContext.SeedLanguages("es", "en");
        _finder = new EntityFrameworkWayOfWorkingByIdFinderAdapter(_dbContext);
    }

    [Fact]
    public async Task FindWayOfWorkingWithItsTranslations()
    {
        var wayId = Guid.NewGuid();
        var spanish = WayOfWorkingTranslationEntityBuilder.Build(wayId, "es", points: ["Una charla", "Un resumen"]);
        var english = WayOfWorkingTranslationEntityBuilder.Build(wayId, "en", "Clear", points: ["A short chat"]);
        var entity = WayOfWorkingEntityBuilder.Build(wayId, 2, "tested", [spanish, english]);
        await _dbContext.WaysOfWorking.AddAsync(entity);
        await _dbContext.WaysOfWorking.AddAsync(WayOfWorkingEntityBuilder.Build(Guid.NewGuid(), 3));
        await _dbContext.SaveChangesAsync();

        var result = await _finder.FindWay(WayOfWorkingIdBuilder.Build(wayId));

        result.Match(
            way =>
            {
                way.GetId().ShouldBe(wayId);
                way.GetPosition().ShouldBe(2);
                way.GetIllustration().ShouldBe("tested");
                way.GetTranslations()
                    .Select(translation => (
                        translation.GetLanguageCode(),
                        translation.GetLabel(),
                        translation.GetPoints()
                    ))
                    .ShouldBe(
                    [
                        (english.LanguageCode, english.Label, english.Points),
                        (spanish.LanguageCode, spanish.Label, spanish.Points)
                    ]);
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task FailWhenTheWayOfWorkingDoesNotExist()
    {
        await _dbContext.WaysOfWorking.AddAsync(WayOfWorkingEntityBuilder.Build(Guid.NewGuid()));
        await _dbContext.SaveChangesAsync();

        var result = await _finder.FindWay(WayOfWorkingIdBuilder.Build());

        result.Match(
            way => way.ShouldBeNull(),
            error => error.ShouldBeOfType<NotFoundError>()
        );
    }

    [Fact]
    public async Task FailWhenUnexpectedErrorOccurs()
    {
        await _dbContext.DisposeAsync();

        var result = await _finder.FindWay(WayOfWorkingIdBuilder.Build());

        result.Match(
            way => way.ShouldBeNull(),
            error => error.ShouldBeOfType<ServiceUnavailableError>()
        );
    }
}
