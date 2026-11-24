using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.WaysOfWorking.Find.Domain.Models;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Ports;
using Portfolio.Api.WaysOfWorking.Shared.Infrastructure.Entities;

namespace Portfolio.Api.WaysOfWorking.Shared.Infrastructure.Adapters;

public class EntityFrameworkWayOfWorkingByIdFinderAdapter(AppDbContext dbContext) : IWayOfWorkingByIdFinderRepository
{
    public async Task<Either<DomainError, WayOfWorkingFound>> FindWay(WayOfWorkingId id)
    {
        try
        {
            var wayId = id.ToGuid();
            var entity = await dbContext.WaysOfWorking
                .Include(way => way.Translations)
                .SingleOrDefaultAsync(way => way.Id == wayId);

            if (entity is null)
            {
                return new NotFoundError($"Way of working '{wayId}' was not found");
            }

            return ToWay(entity);
        }
        catch (Exception error)
        {
            return new ServiceUnavailableError("Database is unavailable, error: " + error.Message, error);
        }
    }

    private static Either<DomainError, WayOfWorkingFound> ToWay(WayOfWorkingEntity entity) =>
        entity.Translations
            .OrderBy(translation => translation.LanguageCode)
            .Select(translation => WayTranslation.CreateUnsafe(
                translation.LanguageCode,
                translation.Label,
                translation.Caption,
                translation.Title,
                translation.Description,
                translation.Points
            ))
            .Sequence()
            .Bind(translations => WayOfWorkingFound.CreateUnsafe(
                entity.Id,
                entity.Position,
                entity.Illustration,
                translations.ToList()
            ));
}
