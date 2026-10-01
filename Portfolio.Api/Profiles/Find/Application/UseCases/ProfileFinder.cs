using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Profiles.Find.Domain.Models;
using Portfolio.Api.Profiles.Find.Domain.Ports;

namespace Portfolio.Api.Profiles.Find.Application.UseCases;

public class ProfileFinder(IProfileFinderRepository repository)
{
    public async Task<Either<Error, ProfileFound>> Execute()
    {
        return await (
            from profile in repository.FindProfile().ToAsync()
            select profile
        );
    }
}
