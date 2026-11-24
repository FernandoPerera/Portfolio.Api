using System.Text.RegularExpressions;
using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.WaysOfWorking.Shared.Domain.Models;

public partial class Illustration
{
    private const int CharacterLimit = 30;

    private readonly string _illustration;

    private Illustration(string illustration)
    {
        _illustration = illustration;
    }

    public static Either<DomainError, Illustration> Create(string illustration)
    {
        if (string.IsNullOrWhiteSpace(illustration))
        {
            return new ValidationError("Illustration cannot be empty");
        }

        var trimmed = illustration.Trim();
        if (trimmed.Length > CharacterLimit)
        {
            return new ValidationError($"The illustration cannot exceed {CharacterLimit} characters");
        }

        if (!KeyFormat().IsMatch(trimmed))
        {
            return new ValidationError(
                "The illustration can only contain lowercase letters, digits and hyphens between words"
            );
        }

        return new Illustration(trimmed);
    }

    public static Either<DomainError, Illustration> CreateUnsafe(string illustration) => new Illustration(illustration);

    public override string ToString() => _illustration;

    [GeneratedRegex("^[a-z0-9]+(-[a-z0-9]+)*$")]
    private static partial Regex KeyFormat();
}
