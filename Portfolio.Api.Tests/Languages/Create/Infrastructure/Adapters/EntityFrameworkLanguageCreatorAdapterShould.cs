using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Languages.Create.Infrastructure.Adapters;
using Portfolio.Api.Profiles.Shared.Infrastructure.Entities;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Languages.Create.Domain.Builders;
using Portfolio.Api.Tests.Shared.Extensions;
using Shouldly;

namespace Portfolio.Api.Tests.Languages.Create.Infrastructure.Adapters;

public class EntityFrameworkLanguageCreatorAdapterShould
{
    private readonly AppDbContext _appDbContext;
    private readonly EntityFrameworkLanguageCreatorAdapter _creator;

    public EntityFrameworkLanguageCreatorAdapterShould()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _appDbContext = new AppDbContext(options);
        _creator = new EntityFrameworkLanguageCreatorAdapter(_appDbContext);
    }

    [Fact]
    public async Task CreateLanguage()
    {
        var languageToBeCreated = LanguageToBeCreatedBuilder.Build();

        var result = await _creator.CreateLanguage(languageToBeCreated);

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                var saved = _appDbContext.Languages.Find(languageToBeCreated.GetLanguageCode());
                saved.ShouldNotBeNull();
                saved.Name.ShouldBe(languageToBeCreated.GetName());
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task KeepExistingLanguageWhenCodeAlreadyExists()
    {
        var existing = new LanguageEntity { LanguageCode = "es", Name = "Spanish" };
        await _appDbContext.Languages.AddAsync(existing);
        await _appDbContext.SaveChangesAsync();

        var result = await _creator.CreateLanguage(
            LanguageToBeCreatedBuilder.Build(languageCode: existing.LanguageCode)
        );

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                _appDbContext.Languages.Count().ShouldBe(1);
                _appDbContext.Languages.Find(existing.LanguageCode)!.Name.ShouldBe(existing.Name);
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task FailWhenErrorOccurs()
    {
        var languageToBeCreated = LanguageToBeCreatedBuilder.Build();
        await _appDbContext.DisposeAsync();

        var result = await _creator.CreateLanguage(languageToBeCreated);

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error => error.Exception.Case.ShouldBeOfType<ServiceUnavailableException>()
        );
    }
}
