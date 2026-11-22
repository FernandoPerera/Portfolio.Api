using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Domain.Ports;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Shared.Extensions;
using Portfolio.Api.Tests.Shared.Infrastructure;
using Portfolio.Api.Tests.WaysOfWorking.Create.Domain.Builders;
using Portfolio.Api.Tests.WaysOfWorking.Create.Infrastructure.Builders;
using Portfolio.Api.Tests.WaysOfWorking.Shared.Domain.Builders;
using Portfolio.Api.WaysOfWorking.Create.Infrastructure.Adapters;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.Create.Infrastructure.Adapters;

public class EntityFrameworkWayOfWorkingCreatorAdapterShould
{
    private readonly AppDbContext _appDbContext;
    private readonly IGeneratorIdentifier _idGenerator = Substitute.For<IGeneratorIdentifier>();
    private readonly EntityFrameworkWayOfWorkingCreatorAdapter _creator;

    public EntityFrameworkWayOfWorkingCreatorAdapterShould()
    {
        var options = PostgresTestDatabase.CreateOptions();

        _appDbContext = new AppDbContext(options);

        _appDbContext.SeedLanguages("es", "en");
        _creator = new EntityFrameworkWayOfWorkingCreatorAdapter(_appDbContext, _idGenerator);
    }

    [Fact]
    public async Task CreateWayOfWorkingInTheFirstPosition()
    {
        var wayId = Guid.NewGuid();
        _idGenerator.Generate().Returns(wayId);
        var spanish = WayTranslationInputBuilder.Build("es", "Claro", points: ["Una charla corta", "Un resumen escrito"]);
        var english = WayTranslationInputBuilder.Build("en", "Clear", points: ["A short chat"]);
        var way = WayOfWorkingToBeCreatedBuilder.Build("tested", [spanish, english]);
        var expected = WayOfWorkingEntityBuilder.Build(
            wayId,
            1,
            "tested",
            [
                WayOfWorkingTranslationEntityBuilder.Build(
                    wayId, "es", spanish.Label, spanish.Caption, spanish.Title, spanish.Description, spanish.Points
                ),
                WayOfWorkingTranslationEntityBuilder.Build(
                    wayId, "en", english.Label, english.Caption, english.Title, english.Description, english.Points
                )
            ]
        );

        var result = await _creator.CreateWay(way);

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                _appDbContext.WaysOfWorking.Include(w => w.Translations).Single().ShouldBeEquivalentTo(expected);
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task CreateWayOfWorkingAfterTheExistingOnes()
    {
        var existingId = Guid.NewGuid();
        var wayId = Guid.NewGuid();
        _appDbContext.WaysOfWorking.Add(WayOfWorkingEntityBuilder.Build(existingId, 3));
        await _appDbContext.SaveChangesAsync();
        _idGenerator.Generate().Returns(wayId);

        var result = await _creator.CreateWay(WayOfWorkingToBeCreatedBuilder.Build());

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                _appDbContext.WaysOfWorking.Single(way => way.Id == wayId).Position.ShouldBe(4);
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task FailWhenErrorOccurs()
    {
        var way = WayOfWorkingToBeCreatedBuilder.Build();
        _idGenerator.Generate().Returns(Guid.NewGuid());
        await _appDbContext.DisposeAsync();

        var result = await _creator.CreateWay(way);

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error => error.ShouldBeOfType<ServiceUnavailableError>()
        );
    }
}
