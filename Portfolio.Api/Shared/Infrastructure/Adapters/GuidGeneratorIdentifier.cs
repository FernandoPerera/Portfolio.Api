using Portfolio.Api.Shared.Domain.Ports;

namespace Portfolio.Api.Shared.Infrastructure.Adapters;

public class GuidGeneratorIdentifier : IGeneratorIdentifier
{
    public Guid Generate() => Guid.NewGuid();
}
