using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Timeline.Create.Domain.Models;

public class Technology
{
    private const int CharacterLimit = 50;

    private readonly string _technology;

    private Technology(string technology)
    {
        _technology = technology;
    }

    public static Either<Error, Technology> Create(string technology)
    {
        if (string.IsNullOrWhiteSpace(technology))
        {
            return Error.New(new ValidationException("Technology cannot be empty"));
        }

        var trimmed = technology.Trim();
        if (trimmed.Length > CharacterLimit)
        {
            return Error.New(new ValidationException($"The technology cannot exceed {CharacterLimit} characters"));
        }

        return new Technology(trimmed);
    }

    public override string ToString() => _technology;
}
