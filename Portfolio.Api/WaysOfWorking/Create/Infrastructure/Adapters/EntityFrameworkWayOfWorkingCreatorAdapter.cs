using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Domain.Ports;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.WaysOfWorking.Create.Domain.Models;
using Portfolio.Api.WaysOfWorking.Create.Domain.Ports;
using Portfolio.Api.WaysOfWorking.Shared.Infrastructure.Entities;

namespace Portfolio.Api.WaysOfWorking.Create.Infrastructure.Adapters;

public class EntityFrameworkWayOfWorkingCreatorAdapter(
    AppDbContext dbContext,
    IGeneratorIdentifier generatorIdentifier
) : IWayOfWorkingCreatorRepository
{
    public async Task<Either<DomainError, Unit>> CreateWay(WayOfWorkingToBeCreated wayOfWorkingToBeCreated)
    {
        try
        {
            var wayId = generatorIdentifier.Generate();
            var lastPosition = await dbContext.WaysOfWorking.MaxAsync(way => (int?)way.Position) ?? 0;
            var entity = new WayOfWorkingEntity
            {
                Id = wayId,
                Position = lastPosition + 1,
                Illustration = wayOfWorkingToBeCreated.GetIllustration(),
                Translations = wayOfWorkingToBeCreated.GetTranslations()
                    .Select(translation => new WayOfWorkingTranslationEntity
                    {
                        WayId = wayId,
                        LanguageCode = translation.GetLanguageCode(),
                        Label = translation.GetLabel(),
                        Caption = translation.GetCaption(),
                        Title = translation.GetTitle(),
                        Description = translation.GetDescription(),
                        Points = translation.GetPoints()
                    })
                    .ToList()
            };

            await dbContext.WaysOfWorking.AddAsync(entity);
            await dbContext.SaveChangesAsync();

            return Unit.Default;
        }
        catch (Exception error)
        {
            return new ServiceUnavailableError("Database is unavailable, error: " + error.Message, error);
        }
    }
}
