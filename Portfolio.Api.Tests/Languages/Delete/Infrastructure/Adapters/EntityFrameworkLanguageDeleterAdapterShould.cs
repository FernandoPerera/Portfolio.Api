using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Languages.Delete.Infrastructure.Adapters;
using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Profiles.Shared.Infrastructure.Entities;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Shared.Extensions;
using Portfolio.Api.Tests.Shared.Infrastructure;
using Shouldly;

namespace Portfolio.Api.Tests.Languages.Delete.Infrastructure.Adapters;

public class EntityFrameworkLanguageDeleterAdapterShould
{
    private readonly AppDbContext _appDbContext;
    private readonly EntityFrameworkLanguageDeleterAdapter _deleter;

    public EntityFrameworkLanguageDeleterAdapterShould()
    {
        var options = PostgresTestDatabase.CreateOptions();

        _appDbContext = new AppDbContext(options);
        _deleter = new EntityFrameworkLanguageDeleterAdapter(_appDbContext);
    }

    [Fact]
    public async Task DeleteTheLanguage()
    {
        var deleted = new LanguageEntity { LanguageCode = "es", Name = "Spanish" };
        var untouched = new LanguageEntity { LanguageCode = "en", Name = "English" };
        await _appDbContext.Languages.AddRangeAsync(deleted, untouched);
        await _appDbContext.SaveChangesAsync();

        var result = await _deleter.DeleteLanguage(ToLanguageCode(deleted.LanguageCode));

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                _appDbContext.Languages.Find(deleted.LanguageCode).ShouldBeNull();
                _appDbContext.Languages.Find(untouched.LanguageCode).ShouldNotBeNull();
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task FailWhenLanguageDoesNotExist()
    {
        var result = await _deleter.DeleteLanguage(ToLanguageCode("fr"));

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error => error.ShouldBeOfType<NotFoundError>()
                .Message.ShouldBe("Language 'fr' was not found")
        );
    }

    [Fact]
    public async Task FailWhenErrorOccurs()
    {
        var languageCode = ToLanguageCode("es");
        await _appDbContext.DisposeAsync();

        var result = await _deleter.DeleteLanguage(languageCode);

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error => error.ShouldBeOfType<ServiceUnavailableError>()
        );
    }

    private static LanguageCode ToLanguageCode(string languageCode) =>
        LanguageCode.Create(languageCode).IfLeft(error => throw new Exception(error.Message));
}
