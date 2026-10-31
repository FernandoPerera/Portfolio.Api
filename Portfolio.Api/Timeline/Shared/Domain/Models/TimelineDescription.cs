using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Timeline.Shared.Domain.Models;

public class TimelineDescription
{
    private const int CharacterLimit = 1000;

    private readonly string _description;

    private TimelineDescription(string description)
    {
        _description = description;
    }

    public static Either<DomainError, TimelineDescription> Create(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return new ValidationError("Description cannot be empty");
        }

        var trimmed = description.Trim();
        if (trimmed.Length > CharacterLimit)
        {
            return new ValidationError($"The description cannot exceed {CharacterLimit} characters");
        }

        return new TimelineDescription(trimmed);
    }

    public static Either<DomainError, TimelineDescription> CreateUnsafe(string description) => new TimelineDescription(description);

    public override string ToString() => _description;
}
