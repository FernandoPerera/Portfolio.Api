using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.WaysOfWorking.Shared.Domain.Models;

public class WayPoints
{
    private const int MinimumPoints = 1;
    private const int MaximumPoints = 5;

    private readonly List<WayPoint> _points;

    private WayPoints(List<WayPoint> points)
    {
        _points = points;
    }

    public static Either<DomainError, WayPoints> Create(List<string> givenPoints)
    {
        if (givenPoints.Count < MinimumPoints || givenPoints.Count > MaximumPoints)
        {
            return new ValidationError($"There must be between {MinimumPoints} and {MaximumPoints} points");
        }

        var hasRepeatedPoints = givenPoints
            .GroupBy(point => point.Trim().ToLowerInvariant())
            .Any(group => group.Count() > 1);
        if (hasRepeatedPoints)
        {
            return new ValidationError("Points cannot be repeated");
        }

        return givenPoints
            .Select(WayPoint.Create)
            .Sequence()
            .Map(points => new WayPoints(points.ToList()));
    }

    public static Either<DomainError, WayPoints> CreateUnsafe(List<string> givenPoints) =>
        givenPoints
            .Select(WayPoint.CreateUnsafe)
            .Sequence()
            .Map(points => new WayPoints(points.ToList()));

    public List<string> GetValues() => _points.Select(point => point.ToString()).ToList();
}
