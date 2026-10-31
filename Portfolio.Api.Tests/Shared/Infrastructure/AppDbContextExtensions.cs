using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Profiles.Shared.Infrastructure.Entities;
using Portfolio.Api.Shared.Infrastructure.Data;

namespace Portfolio.Api.Tests.Shared.Infrastructure;

public static class AppDbContextExtensions
{
    /// <summary>
    /// PostgreSQL enforces the foreign keys from the translations to the languages, which InMemory ignored.
    /// The languages are saved through their own context so the one under test does not track them.
    /// </summary>
    public static void SeedLanguages(this AppDbContext dbContext, params string[] languageCodes)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(dbContext.Database.GetConnectionString()).Options;
        using var seeding = new AppDbContext(options);
        var existing = seeding.Languages.Select(language => language.LanguageCode).ToHashSet();
        seeding.Languages.AddRange(
            languageCodes
                .Where(code => !existing.Contains(code))
                .Select(code => new LanguageEntity { LanguageCode = code, Name = code })
        );
        seeding.SaveChanges();
    }
}
