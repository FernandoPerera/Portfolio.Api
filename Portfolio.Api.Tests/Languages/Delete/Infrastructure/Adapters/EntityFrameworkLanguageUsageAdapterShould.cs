using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Languages.Delete.Infrastructure.Adapters;
using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Profiles.Shared.Infrastructure.Entities;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Profiles.Shared.Infrastructure.Builders;
using Portfolio.Api.Tests.Shared.Extensions;
using Portfolio.Api.Tests.Timeline.Create.Infrastructure.Builders;
using Portfolio.Api.Tests.Shared.Infrastructure;
using Shouldly;

namespace Portfolio.Api.Tests.Languages.Delete.Infrastructure.Adapters;

public class EntityFrameworkLanguageUsageAdapterShould
{
    private const string LanguageCodeInUse = "es";
    private const string UnusedLanguageCode = "en";

    private readonly AppDbContext _appDbContext;
    private readonly EntityFrameworkLanguageUsageAdapter _usage;

    public EntityFrameworkLanguageUsageAdapterShould()
    {
        var options = PostgresTestDatabase.CreateOptions();

        _appDbContext = new AppDbContext(options);

        _appDbContext.SeedLanguages("es", "en");
        _usage = new EntityFrameworkLanguageUsageAdapter(_appDbContext);
    }

    [Fact]
    public async Task AcceptALanguageWithoutTranslations()
    {
        await AddProfileWithTranslation(LanguageCodeInUse);

        var result = await _usage.EnsureIsNotInUse(ToLanguageCode(UnusedLanguageCode));

        result.Match(
            unit => unit.ShouldBeUnit(),
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task RejectALanguageUsedByAProfileTranslation()
    {
        await AddProfileWithTranslation(LanguageCodeInUse);

        var result = await _usage.EnsureIsNotInUse(ToLanguageCode(LanguageCodeInUse));

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error => error.ShouldBeOfType<ConflictError>()
                .Message.ShouldBe("Language 'es' is used by existing translations")
        );
    }

    [Fact]
    public async Task RejectALanguageUsedByATimelineTranslation()
    {
        var entryId = Guid.NewGuid();
        await _appDbContext.TimelineEntries.AddAsync(TimelineEntryEntityBuilder.Build(
            entryId,
            translations: [TimelineEntryTranslationEntityBuilder.Build(entryId, LanguageCodeInUse)]
        ));
        await _appDbContext.SaveChangesAsync();

        var result = await _usage.EnsureIsNotInUse(ToLanguageCode(LanguageCodeInUse));

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error => error.ShouldBeOfType<ConflictError>()
        );
    }

    [Fact]
    public async Task FailWhenErrorOccurs()
    {
        var languageCode = ToLanguageCode(LanguageCodeInUse);
        await _appDbContext.DisposeAsync();

        var result = await _usage.EnsureIsNotInUse(languageCode);

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error => error.ShouldBeOfType<ServiceUnavailableError>()
        );
    }

    private async Task AddProfileWithTranslation(string languageCode)
    {
        var profileId = Guid.NewGuid();
        await _appDbContext.Profiles.AddAsync(ProfileEntityBuilder.Build(
            profileId,
            translations: [ProfileTranslationEntityBuilder.Build(profileId, languageCode)]
        ));
        await _appDbContext.SaveChangesAsync();
    }

    private static LanguageCode ToLanguageCode(string languageCode) =>
        LanguageCode.Create(languageCode).IfLeft(error => throw new Exception(error.Message));
}
