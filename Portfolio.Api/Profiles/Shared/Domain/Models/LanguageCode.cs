using LanguageExt;
using LanguageExt.Common;
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

    public static Either<Error, LanguageCode> Create(string languageCode)
    {
        if (string.IsNullOrWhiteSpace(languageCode))
        {
            return Error.New(new ValidationException("Language code cannot be empty"));
        }

        var trimmed = languageCode.Trim();
        if (trimmed.Length != RequiredLength || !trimmed.All(char.IsLetter))
        {
            return Error.New(new ValidationException("Language code must have 2 letters"));
        }

        return new LanguageCode(trimmed.ToLowerInvariant());
    }

    public override string ToString() => _code;
}
