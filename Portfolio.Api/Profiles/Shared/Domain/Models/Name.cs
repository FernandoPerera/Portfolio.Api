using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Profiles.Shared.Domain.Models;

public class Name
{
    private const int CharacterLimit = 100;
    
    private readonly string _name;

    private Name(string name)
    {
        _name = name;
    }

    public static Either<DomainError, Name> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return new ValidationError("Name cannot be empty");
        }

        if (name.Length > CharacterLimit)
        {
            return new ValidationError("The name cannot exceed 100 characters");
        }
        return new Name(name);
    }

    public override string ToString() => _name;

    public static Either<DomainError, Name> CreateUnsafe(string givenName) => new Name(givenName);
}