using LanguageExt;
using Portfolio.Api.Auth.Shared.Domain.Models;

namespace Portfolio.Api.Auth.Shared.Domain.Ports;

public interface ILoginCodeSaverRepository
{
    Unit Save(LoginCode loginCode);
}
