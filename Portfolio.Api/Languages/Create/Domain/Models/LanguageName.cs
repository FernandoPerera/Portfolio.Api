using LanguageExt;
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

    public static Either<DomainError, LanguageName> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return new ValidationError("Language name cannot be empty");
        }

        var trimmed = name.Trim();
        if (trimmed.Length > CharacterLimit)
        {
            return new ValidationError("The language name cannot exceed 20 characters");
        }

        return new LanguageName(trimmed);
    }

    public static Either<DomainError, LanguageName> CreateUnsafe(string name) => new LanguageName(name);

    public override string ToString() => _name;
}
