using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Profiles.Create.Domain.Models;
using Portfolio.Api.Profiles.Create.Domain.Ports;
using Portfolio.Api.Profiles.Create.Infrastructure.Entities;
using Portfolio.Api.Shared.Domain.Ports;
using Portfolio.Api.Shared.Infrastructure.Data;

namespace Portfolio.Api.Profiles.Create.Infrastructure.Adapters;

public class EntityFrameworkProfileCreatorAdapter(
    AppDbContext dbContext,
    IGeneratorIdentifier generatorIdentifier
) : IProfileCreatorRepository
{
    public async Task<Either<Error, Unit>> CreateProfile(ProfileToBeCreated profileToBeCreated)
    {
        var entity = new ProfileEntity
        {
            Id = generatorIdentifier.Generate(),
            Name = profileToBeCreated.GetName(),
            Presentation = profileToBeCreated.GetPresentation(),
            Role = profileToBeCreated.GetRole(),
            Enterprise = profileToBeCreated.GetEnterprise()
        };

        await dbContext.Profiles.AddAsync(entity);
        await dbContext.SaveChangesAsync();

        return Unit.Default;
    }
}