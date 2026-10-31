using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Profiles.Shared.Infrastructure.Adapters;
using Portfolio.Api.Profiles.Shared.Infrastructure.Entities;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Shared.Extensions;
using Portfolio.Api.Tests.Shared.Infrastructure;
using Shouldly;

namespace Portfolio.Api.Tests.Profiles.Shared.Infrastructure.Adapters;

public class EntityFrameworkLanguageFinderAdapterShould
{
    private readonly AppDbContext _appDbContext;
    private readonly EntityFrameworkLanguageExistenceAdapter _existence;

    public EntityFrameworkLanguageFinderAdapterShould()
    {
        var options = PostgresTestDatabase.CreateOptions();

        _appDbContext = new AppDbContext(options);
        _existence = new EntityFrameworkLanguageExistenceAdapter(_appDbContext);
    }

    [Fact]
    public async Task FindLanguage()
    {
        await _appDbContext.Languages.AddAsync(new LanguageEntity
        {
            LanguageCode = "es",
            Name = "Spanish"
        });
        await _appDbContext.SaveChangesAsync();

        var languageCode = LanguageCode.Create("es")
            .IfLeft(_ => throw new InvalidOperationException());

        var result = await _existence.Exists(languageCode);

        result.Match(
            unit => unit.ShouldBeUnit(),
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task ReturnNotFoundWhenLanguageDoesNotExist()
    {
        var languageCode = LanguageCode.Create("en")
            .IfLeft(_ => throw new InvalidOperationException());

        var result = await _existence.Exists(languageCode);

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error => error.ShouldBeOfType<NotFoundError>()
        );
    }

    [Fact]
    public async Task ReturnServiceUnavailableWhenDatabaseFails()
    {
        var languageCode = LanguageCode.Create("es")
            .IfLeft(_ => throw new InvalidOperationException());
        await _appDbContext.DisposeAsync();

        var result = await _existence.Exists(languageCode);

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error => error.ShouldBeOfType<ServiceUnavailableError>()
        );
    }
}
