using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.WaysOfWorking.Shared.Domain.Models;

public class WayOfWorkingId
{
    private readonly Guid _id;

    private WayOfWorkingId(Guid id)
    {
        _id = id;
    }

    public static Either<DomainError, WayOfWorkingId> Create(Guid id)
    {
        if (id == Guid.Empty)
        {
            return new ValidationError("Way of working id cannot be empty");
        }

        return new WayOfWorkingId(id);
    }

    public Guid ToGuid() => _id;
}
