using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.WaysOfWorking.Shared.Domain.Models;

public class WayTitle
{
    private const int CharacterLimit = 100;

    private readonly string _title;

    private WayTitle(string title)
    {
        _title = title;
    }

    public static Either<DomainError, WayTitle> Create(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return new ValidationError("Title cannot be empty");
        }

        var trimmed = title.Trim();
        if (trimmed.Length > CharacterLimit)
        {
            return new ValidationError($"The title cannot exceed {CharacterLimit} characters");
        }

        return new WayTitle(trimmed);
    }

    public static Either<DomainError, WayTitle> CreateUnsafe(string title) => new WayTitle(title);

    public override string ToString() => _title;
}
