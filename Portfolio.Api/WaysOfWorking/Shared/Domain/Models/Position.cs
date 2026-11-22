using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.WaysOfWorking.Shared.Domain.Models;

public class Position
{
    private readonly int _position;

    private Position(int position)
    {
        _position = position;
    }

    public static Either<DomainError, Position> Create(int position)
    {
        if (position < 1)
        {
            return new ValidationError("Position must be 1 or greater");
        }

        return new Position(position);
    }

    public static Either<DomainError, Position> CreateUnsafe(int position) => new Position(position);

    public int ToInt() => _position;
}
