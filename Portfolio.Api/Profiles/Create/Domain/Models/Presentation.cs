using LanguageExt;
using LanguageExt.Common;

namespace Portfolio.Api.Profiles.Create.Domain.Models;

public class Presentation
{
    private readonly string _value;

    private Presentation(string value) => _value = value;

    public static Either<Error, Presentation> Create(string presentation)
    {
        return new Presentation(presentation);
    }

    public override string ToString() => _value;
}