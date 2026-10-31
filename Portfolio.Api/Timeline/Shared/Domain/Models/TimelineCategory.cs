using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Timeline.Shared.Domain.Models;

public class TimelineCategory
{
    private const string Work = "work";
    private const string Education = "education";

    private readonly string _category;

    private TimelineCategory(string category)
    {
        _category = category;
    }

    public static Either<DomainError, TimelineCategory> Create(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            return new ValidationError("Category cannot be empty");
        }

        var normalized = category.Trim().ToLowerInvariant();
        if (normalized is not (Work or Education))
        {
            return new ValidationError($"Category must be '{Work}' or '{Education}'");
        }

        return new TimelineCategory(normalized);
    }

    public static Either<DomainError, TimelineCategory> CreateUnsafe(string category) => new TimelineCategory(category);

    public override string ToString() => _category;
}
