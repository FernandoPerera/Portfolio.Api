using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Profiles.Find.Domain.Models;

namespace Portfolio.Api.Profiles.Find.Domain.Ports;

public interface IProfileFinderRepository
{
    Task<Either<Error, ProfileFound>> FindProfile();
}
