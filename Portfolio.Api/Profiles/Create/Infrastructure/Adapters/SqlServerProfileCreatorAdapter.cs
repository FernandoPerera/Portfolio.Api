using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Profiles.Create.Domain.Models;
using Portfolio.Api.Profiles.Create.Domain.Ports;

namespace Portfolio.Api.Profiles.Create.Infrastructure.Adapters;

public class SqlServerProfileCreatorAdapter: IProfileCreatorRepository
{
    public Task<Either<Error, Unit>> CreateProfile(ProfileToBeCreated profileToBeCreated)
    {
        throw new NotImplementedException();
    }
}