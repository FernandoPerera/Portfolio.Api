using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.WaysOfWorking.Shared.Domain.Models;

public class WayDescription
{
    private const int CharacterLimit = 500;

    private readonly string _description;

    private WayDescription(string description)
    {
        _description = description;
    }

    public static Either<DomainError, WayDescription> Create(string description)
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

        return new WayDescription(trimmed);
    }

    public static Either<DomainError, WayDescription> CreateUnsafe(string description) =>
        new WayDescription(description);

    public override string ToString() => _description;
}
