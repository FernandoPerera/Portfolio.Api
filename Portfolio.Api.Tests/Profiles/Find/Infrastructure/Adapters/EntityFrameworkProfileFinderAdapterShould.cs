using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Profiles.Create.Infrastructure.Entities;
using Portfolio.Api.Profiles.Find.Infrastructure.Adapters;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Shouldly;

namespace Portfolio.Api.Tests.Profiles.Find.Infrastructure.Adapters;

public class EntityFrameworkProfileFinderAdapterShould: IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly EntityFrameworkProfileFinderAdapter _adapter;

    public EntityFrameworkProfileFinderAdapterShould()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);
        _dbContext.Database.EnsureCreated();

        _adapter = new EntityFrameworkProfileFinderAdapter(_dbContext);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        GC.SuppressFinalize(this);
    }
    
    [Fact]
    public async Task FoundProfile()
    {
        var profileId = Guid.NewGuid();
        var entity = new ProfileEntity
        {
            Id = profileId,
            Name = "Fernando",
            Enterprise = "Lean Mind",
            Translations = new List<ProfileTranslationEntity>
            {
                new()
                {
                    ProfileId = profileId,
                    LanguageCode = "es",
                    Presentation = "Mucho gusto :)",
                    Role = "Beer software architect"
                }
            }
        };
        await _dbContext.Profiles.AddAsync(entity);
        await _dbContext.SaveChangesAsync();

        var result = await _adapter.FindProfile();

        result.Match(
            profile =>
            {
                profile.GetName().ShouldBe("Fernando");
                profile.GetPresentation().ShouldBe("Mucho gusto :)");
                profile.GetRole().ShouldBe("Beer software architect");
                profile.GetEnterprise().ShouldBe("Lean Mind");
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task NotFoundProfileWhenNotExists()
    {
        var result = await _adapter.FindProfile();

        result.Match(
            profile => profile.ShouldBeNull(),
            error => error.Exception.Case.ShouldBeOfType<NotFoundException>()
                .Message.ShouldBe("Profile was not found")
        );
    }

    [Fact]
    public async Task NotFoundProfileWhenUnexpectedErrorOccurs()
    {
        await _dbContext.DisposeAsync();

        var result = await _adapter.FindProfile();

        result.Match(
            profile => profile.ShouldBeNull(),
            error => error.Exception.Case.ShouldBeOfType<ServiceUnavailableException>()
        );
    }
}
