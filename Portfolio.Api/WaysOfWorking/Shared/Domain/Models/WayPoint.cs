using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.WaysOfWorking.Shared.Domain.Models;

public class WayPoint
{
    private const int CharacterLimit = 120;

    private readonly string _point;

    private WayPoint(string point)
    {
        _point = point;
    }

    public static Either<DomainError, WayPoint> Create(string point)
    {
        if (string.IsNullOrWhiteSpace(point))
        {
            return new ValidationError("Point cannot be empty");
        }

        var trimmed = point.Trim();
        if (trimmed.Length > CharacterLimit)
        {
            return new ValidationError($"The point cannot exceed {CharacterLimit} characters");
        }

        return new WayPoint(trimmed);
    }

    public static Either<DomainError, WayPoint> CreateUnsafe(string point) => new WayPoint(point);

    public override string ToString() => _point;
}
