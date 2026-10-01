using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Timeline.Create.Domain.Models;

public class TimelineTitle
{
    private const int CharacterLimit = 100;

    private readonly string _title;

    private TimelineTitle(string title)
    {
        _title = title;
    }

    public static Either<Error, TimelineTitle> Create(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Error.New(new ValidationException("Title cannot be empty"));
        }

        var trimmed = title.Trim();
        if (trimmed.Length > CharacterLimit)
        {
            return Error.New(new ValidationException($"The title cannot exceed {CharacterLimit} characters"));
        }

        return new TimelineTitle(trimmed);
    }

    public override string ToString() => _title;
}
