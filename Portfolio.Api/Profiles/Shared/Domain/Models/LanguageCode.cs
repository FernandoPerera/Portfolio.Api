using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Profiles.Shared.Domain.Models;

public class LanguageCode
{
    private const int RequiredLength = 2;
    private readonly string _code;

    private LanguageCode(string code)
    {
        _code = code;
    }

    public static Either<DomainError, LanguageCode> Create(string languageCode)
    {
        if (string.IsNullOrWhiteSpace(languageCode))
        {
            return new ValidationError("Language code cannot be empty");
        }

        var trimmed = languageCode.Trim();
        if (trimmed.Length != RequiredLength || !trimmed.All(char.IsLetter))
        {
            return new ValidationError("Language code must have 2 letters");
        }

        return new LanguageCode(trimmed.ToLowerInvariant());
    }

    public static Either<DomainError, LanguageCode> CreateUnsafe(string languageCode) => new LanguageCode(languageCode);

    public override string ToString() => _code;
}
