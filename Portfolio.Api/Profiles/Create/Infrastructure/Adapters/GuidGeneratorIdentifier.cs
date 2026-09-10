using Portfolio.Api.Shared.Domain.Ports;

namespace Portfolio.Api.Profiles.Create.Infrastructure.Adapters;

public class GuidGeneratorIdentifier: IGeneratorIdentifier
{
    public Guid Generate()
    {
        throw new NotImplementedException();
    }
}