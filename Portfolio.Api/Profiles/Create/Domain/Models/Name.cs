using LanguageExt;
using LanguageExt.Common;

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
        return new Name(name);
    }

    public override string ToString() => _name;
}