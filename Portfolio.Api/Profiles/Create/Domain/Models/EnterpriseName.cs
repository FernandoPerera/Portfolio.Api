using LanguageExt;
using LanguageExt.Common;

namespace Portfolio.Api.Profiles.Create.Domain.Models;

public class EnterpriseName
{
    private readonly string? _value;

    private EnterpriseName(string? value) => _value = value;

    public static Either<Error, EnterpriseName> Create(string? enterpriseName)
    {
        return new EnterpriseName(enterpriseName);
    }

    public override string ToString() => _value ?? string.Empty;
}