using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Languages.Find.Infrastructure.Adapters;
using Portfolio.Api.Profiles.Shared.Infrastructure.Entities;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Shouldly;

namespace Portfolio.Api.Tests.Languages.Find.Infrastructure.Adapters;

public class EntityFrameworkLanguageFinderAdapterShould
{
    private readonly AppDbContext _dbContext;
    private readonly EntityFrameworkLanguageFinderAdapter _adapter;

    public EntityFrameworkLanguageFinderAdapterShould()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);
        _adapter = new EntityFrameworkLanguageFinderAdapter(_dbContext);
    }

    [Fact]
    public async Task FindLanguagesOrderedByCode()
    {
        await _dbContext.Languages.AddRangeAsync(
            new LanguageEntity { LanguageCode = "fr", Name = "Français" },
            new LanguageEntity { LanguageCode = "en", Name = "English" },
            new LanguageEntity { LanguageCode = "es", Name = "Español" }
        );
        await _dbContext.SaveChangesAsync();

        var result = await _adapter.FindLanguages();

        result.Match(
            languages => languages
                .Select(language => (language.GetLanguageCode(), language.GetName()))
                .ShouldBe([("en", "English"), ("es", "Español"), ("fr", "Français")]),
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task FindNoLanguagesWhenThereAreNone()
    {
        var result = await _adapter.FindLanguages();

        result.Match(
            languages => languages.ShouldBeEmpty(),
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task FailWhenUnexpectedErrorOccurs()
    {
        await _dbContext.DisposeAsync();

        var result = await _adapter.FindLanguages();

        result.Match(
            languages => languages.ShouldBeNull(),
            error => error.Exception.Case.ShouldBeOfType<ServiceUnavailableException>()
        );
    }
}
