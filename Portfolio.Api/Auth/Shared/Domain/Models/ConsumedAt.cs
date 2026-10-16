using LanguageExt;
using LanguageExt.Common;

namespace Portfolio.Api.Auth.Shared.Domain.Models;

public class ConsumedAt
{
    private readonly Option<DateTimeOffset> _consumedAt;

    private ConsumedAt(Option<DateTimeOffset> consumedAt)
    {
        _consumedAt = consumedAt;
    }

    public static Either<Error, ConsumedAt> Create(DateTimeOffset consumedAt) =>
        new ConsumedAt(Option<DateTimeOffset>.Some(consumedAt));

    public static Either<Error, ConsumedAt> NotConsumed() => new ConsumedAt(Option<DateTimeOffset>.None);

    public static Either<Error, ConsumedAt> CreateUnsafe(DateTimeOffset? consumedAt) =>
        new ConsumedAt(consumedAt is { } value ? Option<DateTimeOffset>.Some(value) : Option<DateTimeOffset>.None);

    public bool IsConsumed() => _consumedAt.IsSome;

    public bool IsAtOrAfter(DateTimeOffset moment) => _consumedAt.Match(consumedAt => consumedAt >= moment, () => false);

    public DateTimeOffset? GetValue() => _consumedAt.MatchUnsafe(consumedAt => (DateTimeOffset?)consumedAt, () => null);
}
