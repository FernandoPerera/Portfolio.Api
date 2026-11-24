using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.WaysOfWorking.Shared.Domain.Models;

public class WayLabel
{
    private const int CharacterLimit = 30;

    private readonly string _label;

    private WayLabel(string label)
    {
        _label = label;
    }

    public static Either<DomainError, WayLabel> Create(string label)
    {
        if (string.IsNullOrWhiteSpace(label))
        {
            return new ValidationError("Label cannot be empty");
        }

        var trimmed = label.Trim();
        if (trimmed.Length > CharacterLimit)
        {
            return new ValidationError($"The label cannot exceed {CharacterLimit} characters");
        }

        return new WayLabel(trimmed);
    }

    public static Either<DomainError, WayLabel> CreateUnsafe(string label) => new WayLabel(label);

    public override string ToString() => _label;
}
