using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;

namespace Portfolio.Api.WaysOfWorking.Update.Domain.Models;

public class WayOfWorkingToBeUpdated
{
    private readonly WayOfWorkingId _id;
    private readonly Illustration _illustration;

    private WayOfWorkingToBeUpdated(WayOfWorkingId id, Illustration illustration)
    {
        _id = id;
        _illustration = illustration;
    }

    public static Either<DomainError, WayOfWorkingToBeUpdated> Create(Guid givenId, string givenIllustration) =>
        from id in WayOfWorkingId.Create(givenId)
        from illustration in Illustration.Create(givenIllustration)
        select new WayOfWorkingToBeUpdated(id, illustration);

    public Guid GetId() => _id.ToGuid();

    public string GetIllustration() => _illustration.ToString();
}
