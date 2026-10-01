using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Shared.Domain.Errors;

namespace Ci.Languages.Create.Domain.Models;

public class LanguageName
{
    private const int CharacterLimit = 20;

    private readonly string _name;

    private LanguageName(string name)
    {
        _name = name;
    }

    public static Either<Error, LanguageName> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Error.New(new ValidationException("Language name cannot be empty"));
        }

        var trimmed = name.Trim();
        if (trimmed.Length > CharacterLimit)
        {
            return Error.New(new ValidationException("The language name cannot exceed 20 characters"));
        }

        return new LanguageName(trimmed);
    }

    public override string ToString() => _name;
}
