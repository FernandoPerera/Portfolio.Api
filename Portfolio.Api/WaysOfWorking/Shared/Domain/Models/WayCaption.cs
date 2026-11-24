using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.WaysOfWorking.Shared.Domain.Models;

public class WayCaption
{
    private const int CharacterLimit = 60;

    private readonly string _caption;

    private WayCaption(string caption)
    {
        _caption = caption;
    }

    public static Either<DomainError, WayCaption> Create(string caption)
    {
        if (string.IsNullOrWhiteSpace(caption))
        {
            return new ValidationError("Caption cannot be empty");
        }

        var trimmed = caption.Trim();
        if (trimmed.Length > CharacterLimit)
        {
            return new ValidationError($"The caption cannot exceed {CharacterLimit} characters");
        }

        return new WayCaption(trimmed);
    }

    public static Either<DomainError, WayCaption> CreateUnsafe(string caption) => new WayCaption(caption);

    public override string ToString() => _caption;
}
