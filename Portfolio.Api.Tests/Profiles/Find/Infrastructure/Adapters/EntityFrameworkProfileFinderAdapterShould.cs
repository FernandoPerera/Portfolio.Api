using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Profiles.Create.Infrastructure.Entities;
using Portfolio.Api.Profiles.Find.Infrastructure.Adapters;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Profiles.Shared.Infrastructure.Builders;
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
        var spanish = ProfileTranslationEntityBuilder.Build(profileId);
        var english = ProfileTranslationEntityBuilder.Build(profileId, "en");
        var entity = ProfileEntityBuilder.Build(profileId, translations: [spanish, english]);
        await _dbContext.Profiles.AddAsync(entity);
        await _dbContext.SaveChangesAsync();

        var result = await _adapter.FindProfile();

        result.Match(
            profile =>
            {
                profile.GetName().ShouldBe(entity.Name);
                profile.GetEnterprise().ShouldBe(entity.Enterprise);
                profile.GetTranslations()
                    .Select(translation => (
                        translation.GetLanguageCode(),
                        translation.GetPresentation(),
                        translation.GetRole()
                    ))
                    .ShouldBe(
                    [
                        (english.LanguageCode, english.Presentation, english.Role),
                        (spanish.LanguageCode, spanish.Presentation, spanish.Role)
                    ]);
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task FoundProfileWithoutTranslations()
    {
        await _dbContext.Profiles.AddAsync(ProfileEntityBuilder.Build(Guid.NewGuid()));
        await _dbContext.SaveChangesAsync();

        var result = await _adapter.FindProfile();

        result.Match(
            profile => profile.GetTranslations().ShouldBeEmpty(),
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
