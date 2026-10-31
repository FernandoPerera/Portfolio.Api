using LanguageExt;
using Portfolio.Api.Profiles.Create.Domain.Models;
using Portfolio.Api.Profiles.Create.Domain.Ports;
using Portfolio.Api.Profiles.Create.Infrastructure.Entities;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Shared.Domain.Ports;
using Portfolio.Api.Shared.Infrastructure.Data;

namespace Portfolio.Api.Profiles.Create.Infrastructure.Adapters;

public class EntityFrameworkProfileCreatorAdapter(
    AppDbContext dbContext,
    IGeneratorIdentifier generatorIdentifier
) : IProfileCreatorRepository
{
    public async Task<Either<DomainError, Unit>> CreateProfile(ProfileToBeCreated profileToBeCreated)
    {
        try
        {
            var profileId = generatorIdentifier.Generate();
            var entity = new ProfileEntity
            {
                Id = profileId,
                Name = profileToBeCreated.GetName(),
                Enterprise = profileToBeCreated.GetEnterprise(),
                Translations = new List<ProfileTranslationEntity>
                {
                    new()
                    {
                        ProfileId = profileId,
                        LanguageCode = profileToBeCreated.GetLanguageCode(),
                        Presentation = profileToBeCreated.GetPresentation(),
                        Role = profileToBeCreated.GetRole()
                    }
                }
            };

            await dbContext.Profiles.AddAsync(entity);
            await dbContext.SaveChangesAsync();

            return Unit.Default;
        }
        catch (Exception error)
        {
            return new ServiceUnavailableError("Database is unavailable, error: " + error.Message, error);
        }
    }
}