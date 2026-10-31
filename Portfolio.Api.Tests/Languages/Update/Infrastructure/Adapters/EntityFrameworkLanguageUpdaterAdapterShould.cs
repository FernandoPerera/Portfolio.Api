using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Languages.Update.Infrastructure.Adapters;
using Portfolio.Api.Profiles.Shared.Infrastructure.Entities;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Languages.Update.Domain.Builders;
using Portfolio.Api.Tests.Shared.Extensions;
using Portfolio.Api.Tests.Shared.Infrastructure;
using Shouldly;

namespace Portfolio.Api.Tests.Languages.Update.Infrastructure.Adapters;

public class EntityFrameworkLanguageUpdaterAdapterShould
{
    private readonly AppDbContext _appDbContext;
    private readonly EntityFrameworkLanguageUpdaterAdapter _updater;

    public EntityFrameworkLanguageUpdaterAdapterShould()
    {
        var options = PostgresTestDatabase.CreateOptions();

        _appDbContext = new AppDbContext(options);
        _updater = new EntityFrameworkLanguageUpdaterAdapter(_appDbContext);
    }

    [Fact]
    public async Task UpdateLanguageName()
    {
        const string givenName = "Español";
        var untouched = new LanguageEntity { LanguageCode = "en", Name = "English" };
        var existing = new LanguageEntity { LanguageCode = "es", Name = "Spanish" };
        await _appDbContext.Languages.AddRangeAsync(existing, untouched);
        await _appDbContext.SaveChangesAsync();

        var result = await _updater.UpdateLanguage(
            LanguageToBeUpdatedBuilder.Build(languageCode: existing.LanguageCode, name: givenName)
        );

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                _appDbContext.Languages.Find(existing.LanguageCode)!.Name.ShouldBe(givenName);
                _appDbContext.Languages.Find(untouched.LanguageCode)!.Name.ShouldBe(untouched.Name);
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task FailWhenLanguageDoesNotExist()
    {
        var result = await _updater.UpdateLanguage(LanguageToBeUpdatedBuilder.Build(languageCode: "fr"));

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error => error.ShouldBeOfType<NotFoundError>()
                .Message.ShouldBe("Language 'fr' was not found")
        );
    }

    [Fact]
    public async Task FailWhenErrorOccurs()
    {
        var languageToBeUpdated = LanguageToBeUpdatedBuilder.Build();
        await _appDbContext.DisposeAsync();

        var result = await _updater.UpdateLanguage(languageToBeUpdated);

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error => error.ShouldBeOfType<ServiceUnavailableError>()
        );
    }
}
