using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Profiles.Create.Infrastructure.Entities;
using Portfolio.Api.Profiles.Find.Domain.Models;
using Portfolio.Api.Profiles.Update.Infrastructure.Adapters;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Profiles.Find.Domain.Builders;
using Portfolio.Api.Tests.Profiles.Shared.Infrastructure.Builders;
using Portfolio.Api.Tests.Profiles.Update.Domain.Builders;
using Portfolio.Api.Tests.Shared.Extensions;
using Portfolio.Api.Tests.Shared.Infrastructure;
using Shouldly;

namespace Portfolio.Api.Tests.Profiles.Update.Infrastructure.Adapters;

public class EntityFrameworkProfileUpdaterAdapterShould : IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly EntityFrameworkProfileUpdaterAdapter _adapter;

    public EntityFrameworkProfileUpdaterAdapterShould()
    {
        var options = PostgresTestDatabase.CreateOptions();

        _dbContext = new AppDbContext(options);

        _dbContext.SeedLanguages("es", "en", "fr");
        _dbContext.Database.EnsureCreated();

        _adapter = new EntityFrameworkProfileUpdaterAdapter(_dbContext);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task UpdateProfile()
    {
        const string givenLanguageCode = "es";
        const string givenName = "Fernando Perera";
        const string givenEnterprise = "Nextret";
        const string givenRole = "Backend developer";
        const string givenPresentation = "Hola, soy Fernando";
        var profileId = Guid.NewGuid();
        var spanish = ProfileTranslationEntityBuilder.Build(profileId, givenLanguageCode);
        var english = ProfileTranslationEntityBuilder.Build(profileId, "en", "Nice to meet you", "Software Engineer");
        await _dbContext.Profiles.AddAsync(ProfileEntityBuilder.Build(profileId, translations: [spanish, english]));
        await _dbContext.SaveChangesAsync();
        var profileToBeUpdated = ProfileToBeUpdatedBuilder.Build(
            name: givenName,
            enterprise: givenEnterprise,
            jobRole: givenRole,
            presentation: givenPresentation,
            languageCode: givenLanguageCode
        );

        var result = await _adapter.UpdateProfile(profileToBeUpdated);

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                var stored = _dbContext.Profiles.AsNoTracking().Include(p => p.Translations).Single();
                stored.Name.ShouldBe(givenName);
                stored.Enterprise.ShouldBe(givenEnterprise);
                var updated = stored.Translations.Single(t => t.LanguageCode == givenLanguageCode);
                updated.Role.ShouldBe(givenRole);
                updated.Presentation.ShouldBe(givenPresentation);
                var untouched = stored.Translations.Single(t => t.LanguageCode == english.LanguageCode);
                untouched.Role.ShouldBe(english.Role);
                untouched.Presentation.ShouldBe(english.Presentation);
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task UpdateOnlyTheProvidedFields()
    {
        const string givenRole = "Backend developer";
        var profileId = Guid.NewGuid();
        var translation = ProfileTranslationEntityBuilder.Build(profileId);
        var entity = ProfileEntityBuilder.Build(profileId, translations: [translation]);
        await _dbContext.Profiles.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        var profileToBeUpdated = ProfileToBeUpdatedBuilder.Build(
            name: null,
            enterprise: null,
            jobRole: givenRole,
            presentation: null,
            languageCode: translation.LanguageCode,
            current: CurrentProfile(entity, translation)
        );

        var result = await _adapter.UpdateProfile(profileToBeUpdated);

        result.Match(
            _ =>
            {
                var stored = _dbContext.Profiles.AsNoTracking().Include(p => p.Translations).Single();
                stored.Name.ShouldBe(entity.Name);
                stored.Enterprise.ShouldBe(entity.Enterprise);
                stored.Translations.Single().Role.ShouldBe(givenRole);
                stored.Translations.Single().Presentation.ShouldBe(translation.Presentation);
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task KeepTheProfileWhenThereAreNoFieldsToUpdate()
    {
        var profileId = Guid.NewGuid();
        var translation = ProfileTranslationEntityBuilder.Build(profileId);
        var entity = ProfileEntityBuilder.Build(profileId, translations: [translation]);
        await _dbContext.Profiles.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        var profileToBeUpdated = ProfileToBeUpdatedBuilder.Build(
            name: null,
            enterprise: null,
            jobRole: null,
            presentation: null,
            languageCode: translation.LanguageCode,
            current: CurrentProfile(entity, translation)
        );

        var result = await _adapter.UpdateProfile(profileToBeUpdated);

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                var stored = _dbContext.Profiles.AsNoTracking().Include(p => p.Translations).Single();
                stored.Name.ShouldBe(entity.Name);
                stored.Enterprise.ShouldBe(entity.Enterprise);
                stored.Translations.Single().Role.ShouldBe(translation.Role);
                stored.Translations.Single().Presentation.ShouldBe(translation.Presentation);
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task NotFoundProfileWhenNotExists()
    {
        var result = await _adapter.UpdateProfile(ProfileToBeUpdatedBuilder.Build());

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error => error.ShouldBeOfType<NotFoundError>()
                .Message.ShouldBe("Profile was not found")
        );
    }

    [Fact]
    public async Task NotFoundTranslationWhenLanguageIsNotInTheProfile()
    {
        var profileId = Guid.NewGuid();
        var translation = ProfileTranslationEntityBuilder.Build(profileId);
        await _dbContext.Profiles.AddAsync(ProfileEntityBuilder.Build(profileId, translations: [translation]));
        await _dbContext.SaveChangesAsync();

        var result = await _adapter.UpdateProfile(ProfileToBeUpdatedBuilder.Build(languageCode: "fr"));

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error => error.ShouldBeOfType<NotFoundError>()
                .Message.ShouldBe("Profile translation for language 'fr' was not found")
        );
    }

    [Fact]
    public async Task FailWhenUnexpectedErrorOccurs()
    {
        await _dbContext.DisposeAsync();

        var result = await _adapter.UpdateProfile(ProfileToBeUpdatedBuilder.Build());

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error => error.ShouldBeOfType<ServiceUnavailableError>()
        );
    }

    private static ProfileFound CurrentProfile(ProfileEntity entity, ProfileTranslationEntity translation) =>
        ProfileFoundBuilder.Build(
            entity.Name,
            entity.Enterprise,
            [ProfileTranslationFoundBuilder.Build(translation.LanguageCode, translation.Presentation, translation.Role)]
        );
}
