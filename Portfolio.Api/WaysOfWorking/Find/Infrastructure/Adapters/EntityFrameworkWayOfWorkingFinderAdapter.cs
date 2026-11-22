using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.WaysOfWorking.Find.Domain.Models;
using Portfolio.Api.WaysOfWorking.Find.Domain.Ports;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;
using Portfolio.Api.WaysOfWorking.Shared.Infrastructure.Entities;

namespace Portfolio.Api.WaysOfWorking.Find.Infrastructure.Adapters;

public class EntityFrameworkWayOfWorkingFinderAdapter(AppDbContext dbContext) : IWayOfWorkingFinderRepository
{
    public async Task<Either<DomainError, List<WayOfWorkingFound>>> FindWays()
    {
        try
        {
            var entities = await dbContext.WaysOfWorking
                .Include(way => way.Translations)
                .OrderBy(way => way.Position)
                .ToListAsync();

            return entities
                .Select(ToWay)
                .Sequence()
                .Map(ways => ways.ToList());
        }
        catch (Exception error)
        {
            return new ServiceUnavailableError("Database is unavailable, error: " + error.Message, error);
        }
    }

    private static Either<DomainError, WayOfWorkingFound> ToWay(WayOfWorkingEntity entity) =>
        entity.Translations
            .OrderBy(translation => translation.LanguageCode)
            .Select(ToTranslation)
            .Sequence()
            .Bind(translations => WayOfWorkingFound.CreateUnsafe(
                entity.Id,
                entity.Position,
                entity.Illustration,
                translations.ToList()
            ));

    private static Either<DomainError, WayTranslation> ToTranslation(WayOfWorkingTranslationEntity entity) =>
        WayTranslation.CreateUnsafe(
            entity.LanguageCode,
            entity.Label,
            entity.Caption,
            entity.Title,
            entity.Description,
            entity.Points
        );
}
