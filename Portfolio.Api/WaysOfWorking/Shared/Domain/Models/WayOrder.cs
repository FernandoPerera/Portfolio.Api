using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.WaysOfWorking.Shared.Domain.Models;

public class WayOrder
{
    private readonly List<WayOfWorkingId> _ids;

    private WayOrder(List<WayOfWorkingId> ids)
    {
        _ids = ids;
    }

    public static Either<DomainError, WayOrder> Create(List<Guid> givenIds)
    {
        if (givenIds.Count == 0)
        {
            return new ValidationError("The order needs at least one way of working");
        }

        if (givenIds.Distinct().Count() != givenIds.Count)
        {
            return new ValidationError("The order cannot repeat a way of working");
        }

        return givenIds
            .Select(WayOfWorkingId.Create)
            .Sequence()
            .Map(ids => new WayOrder(ids.ToList()));
    }

    public List<Guid> GetValues() => _ids.Select(id => id.ToGuid()).ToList();
}
