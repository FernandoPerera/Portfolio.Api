using LanguageExt;
using Portfolio.Api.Profiles.Find.Domain.Models;
using Portfolio.Api.Profiles.Find.Domain.Ports;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Profiles.Find.Application.UseCases;

public class ProfileFinder(IProfileFinderRepository repository)
{
    public async Task<Either<DomainError, ProfileFound>> Execute()
    {
        return await (
            from profile in repository.FindProfile().ToAsync()
            select profile
        );
    }
}
