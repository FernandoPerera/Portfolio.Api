using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Profiles.Create.Domain.Models;

public class Name
{
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
        return new Name(name);
    }

    public override string ToString() => _name;
}