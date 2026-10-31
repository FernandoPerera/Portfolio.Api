using LanguageExt;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Portfolio.Api.Profiles.Create.Infrastructure.Adapters;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Domain.Ports;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Profiles.Create.Domain.Builders;
using Portfolio.Api.Tests.Shared.Extensions;
using Shouldly;

namespace Portfolio.Api.Tests.Profiles.Create.Infrastructure.Adapters;

public class EntityFrameworkProfileCreatorAdapterShould
{
    private readonly AppDbContext _appDbContext;
    private readonly IGeneratorIdentifier _idGenerator = Substitute.For<IGeneratorIdentifier>();
    private readonly EntityFrameworkProfileCreatorAdapter _creator;

    public EntityFrameworkProfileCreatorAdapterShould()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "example_db")
            .Options;

        _appDbContext = new AppDbContext(options);
        _creator = new EntityFrameworkProfileCreatorAdapter(_appDbContext, _idGenerator);
    }

    [Fact]
    public async Task CreateProfile()
    {
        var profileToBeCreated = ProfileToBeCreatedBuilder.Build();
        var profileId = Guid.NewGuid();
        _idGenerator.Generate().Returns(profileId);

        var result = await _creator.CreateProfile(profileToBeCreated);

        await result.MatchAsync(
            RightAsync: async unit =>
            {
                unit.ShouldBeUnit();
                var saved = await _appDbContext.Profiles.Include(p => p.Translations)
                    .FirstOrDefaultAsync(profile => profile.Id == profileId);
                saved.ShouldNotBeNull();
                saved.Name.ShouldBe(profileToBeCreated.GetName());
                saved.Enterprise.ShouldBe(profileToBeCreated.GetEnterprise());
                saved.Translations.Count.ShouldBe(1);
                var translation = saved.Translations.First();
                translation.LanguageCode.ShouldBe(profileToBeCreated.GetLanguageCode());
                translation.Presentation.ShouldBe(profileToBeCreated.GetPresentation());
                translation.Role.ShouldBe(profileToBeCreated.GetRole());
                return Unit.Default;
            },
            Left: error =>
            {
                error.ShouldBeNull();
                return Unit.Default;
            }
        );
    }

    [Fact]
    public async Task NotCreateProfileWhenErrorOccurs()
    {
        var profileToBeCreated = ProfileToBeCreatedBuilder.Build();
        var profileId = Guid.NewGuid();
        _idGenerator.Generate().Returns(profileId);
        await _appDbContext.DisposeAsync();

        var result = await _creator.CreateProfile(profileToBeCreated);

        result.Match(
            Right: unit => unit.ShouldBeEquivalentTo(null),
            Left: error => error.ShouldBeOfType<ServiceUnavailableError>()
        );
    }
}