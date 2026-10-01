using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Timeline.Create.Domain.Models;

public class TimelineDescription
{
    private const int CharacterLimit = 1000;

    private readonly string _description;

    private TimelineDescription(string description)
    {
        _description = description;
    }

    public static Either<Error, TimelineDescription> Create(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return Error.New(new ValidationException("Description cannot be empty"));
        }

        var trimmed = description.Trim();
        if (trimmed.Length > CharacterLimit)
        {
            return Error.New(new ValidationException($"The description cannot exceed {CharacterLimit} characters"));
        }

        return new TimelineDescription(trimmed);
    }

    public override string ToString() => _description;
}
