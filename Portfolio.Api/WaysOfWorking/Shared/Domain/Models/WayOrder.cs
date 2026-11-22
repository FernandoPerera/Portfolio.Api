using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.WaysOfWorking.Find.Domain.Models;

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

    public Either<DomainError, Unit> EnsureCoversExactly(List<WayOfWorkingFound> existingWays)
    {
        var existingIds = existingWays.Select(way => way.GetId()).ToHashSet();
        if (!existingIds.SetEquals(GetValues()))
        {
            return new ValidationError("The order must contain exactly the existing ways of working");
        }

        return Unit.Default;
    }
}
