using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Shared.Extensions;
using Portfolio.Api.Tests.Shared.Infrastructure;
using Portfolio.Api.Tests.WaysOfWorking.AddTranslation.Domain.Builders;
using Portfolio.Api.Tests.WaysOfWorking.Create.Infrastructure.Builders;
using Portfolio.Api.WaysOfWorking.AddTranslation.Infrastructure.Adapters;
using Portfolio.Api.WaysOfWorking.Shared.Infrastructure.Entities;
using Shouldly;

namespace Portfolio.Api.Tests.WaysOfWorking.AddTranslation.Infrastructure.Adapters;

public class EntityFrameworkWayTranslationAdderAdapterShould
{
    private readonly AppDbContext _dbContext;
    private readonly EntityFrameworkWayTranslationAdderAdapter _adder;

    public EntityFrameworkWayTranslationAdderAdapterShould()
    {
        var options = PostgresTestDatabase.CreateOptions();

        _dbContext = new AppDbContext(options);

        _dbContext.SeedLanguages("es", "en");
        _adder = new EntityFrameworkWayTranslationAdderAdapter(_dbContext);
    }

    [Fact]
    public async Task AddTheTranslation()
    {
        var wayId = Guid.NewGuid();
        var existing = WayOfWorkingTranslationEntityBuilder.Build(wayId, "es");
        await _dbContext.WaysOfWorking.AddAsync(WayOfWorkingEntityBuilder.Build(wayId, translations: [existing]));
        await _dbContext.SaveChangesAsync();
        var translation = WayTranslationToBeAddedBuilder.Build(wayId, "en", "Clear", points: ["A short chat", "A summary"]);

        var result = await _adder.AddTranslation(translation);

        result.Match(
            unit =>
            {
                unit.ShouldBeUnit();
                AssertStoredTranslations(
                    WayOfWorkingTranslationEntityBuilder.Build(
                        wayId,
                        "en",
                        "Clear",
                        "No jargon",
                        "I explain it without jargon",
                        "Before writing a line we talk in plain words.",
                        ["A short chat", "A summary"]
                    ),
                    existing
                );
            },
            error => error.ShouldBeNull()
        );
    }

    [Fact]
    public async Task FailWhenErrorOccurs()
    {
        var translation = WayTranslationToBeAddedBuilder.Build();
        await _dbContext.DisposeAsync();

        var result = await _adder.AddTranslation(translation);

        result.Match(
            unit => unit.ShouldBeEquivalentTo(null),
            error => error.ShouldBeOfType<ServiceUnavailableError>()
        );
    }

    private void AssertStoredTranslations(params WayOfWorkingTranslationEntity[] expected) =>
        ToComparable(_dbContext.WayOfWorkingTranslations).ShouldBeEquivalentTo(ToComparable(expected));

    private static List<object> ToComparable(IEnumerable<WayOfWorkingTranslationEntity> translations) =>
        translations
            .OrderBy(translation => translation.LanguageCode)
            .Select(translation => (object)new
            {
                translation.WayId,
                translation.LanguageCode,
                translation.Label,
                translation.Caption,
                translation.Title,
                translation.Description,
                translation.Points
            })
            .ToList();
}
