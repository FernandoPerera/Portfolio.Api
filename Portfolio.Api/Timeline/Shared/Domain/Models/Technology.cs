using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Timeline.Shared.Domain.Models;

public class Technology
{
    private const int CharacterLimit = 50;

    private readonly string _technology;

    private Technology(string technology)
    {
        _technology = technology;
    }

    public static Either<DomainError, Technology> Create(string technology)
    {
        if (string.IsNullOrWhiteSpace(technology))
        {
            return new ValidationError("Technology cannot be empty");
        }

        var trimmed = technology.Trim();
        if (trimmed.Length > CharacterLimit)
        {
            return new ValidationError($"The technology cannot exceed {CharacterLimit} characters");
        }

        return new Technology(trimmed);
    }

    public static Either<DomainError, Technology> CreateUnsafe(string technology) => new Technology(technology);

    public override string ToString() => _technology;
}
