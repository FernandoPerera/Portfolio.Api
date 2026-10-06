using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Profiles.Find.Domain.Ports;
using Portfolio.Api.Profiles.Update.Application.Dtos;
using Portfolio.Api.Profiles.Update.Domain.Models;
using Portfolio.Api.Profiles.Update.Domain.Ports;

namespace Portfolio.Api.Profiles.Update.Application.UseCases;

public class ProfileUpdater(IProfileFinderRepository finder, IProfileUpdaterRepository updater)
{
    public async Task<Either<Error, Unit>> Execute(ProfileUpdaterInfo profileUpdaterInfo)
    {
        return await (
            from current in finder.FindProfile().ToAsync()
            from profile in ProfileToBeUpdated.Create(
                current,
                profileUpdaterInfo.Name,
                profileUpdaterInfo.EnterpriseName,
                profileUpdaterInfo.JobRole,
                profileUpdaterInfo.Presentation,
                profileUpdaterInfo.LanguageCode
            ).ToAsync()
            from result in updater.UpdateProfile(profile).ToAsync()
            select result
        );
    }
}
