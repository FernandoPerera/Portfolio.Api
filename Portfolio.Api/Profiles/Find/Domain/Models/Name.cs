using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Profiles.Find.Domain.Models;

public class Name
{
    private const int CharacterLimit = 100;
    
    private readonly string _name;

    private Name(string name)
    {
        _name = name;
    }

    public static Either<Error, Name> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Error.New(new ValidationException("Name cannot be empty"));
        }

        if (name.Length > CharacterLimit)
        {
            return Error.New(new ValidationException("The name cannot exceed 100 characters"));
        }
        return new Name(name);
    }

    public override string ToString() => _name;

    public static Either<Error, Name> CreateUnsafe(string givenName) => new Name(givenName);
}
