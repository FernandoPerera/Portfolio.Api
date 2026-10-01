using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Timeline.Create.Domain.Models;

public class TimelineCategory
{
    private const string Work = "work";
    private const string Education = "education";

    private readonly string _category;

    private TimelineCategory(string category)
    {
        _category = category;
    }

    public static Either<Error, TimelineCategory> Create(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            return Error.New(new ValidationException("Category cannot be empty"));
        }

        var normalized = category.Trim().ToLowerInvariant();
        if (normalized is not (Work or Education))
        {
            return Error.New(new ValidationException($"Category must be '{Work}' or '{Education}'"));
        }

        return new TimelineCategory(normalized);
    }

    public override string ToString() => _category;
}
