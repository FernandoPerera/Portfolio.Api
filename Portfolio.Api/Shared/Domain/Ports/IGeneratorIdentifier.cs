namespace Portfolio.Api.Shared.Domain.Ports;

public interface IGeneratorIdentifier
{
    Guid Generate();
}