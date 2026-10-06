using LanguageExt;
using LanguageExt.Common;

namespace Portfolio.Api.Profiles.Shared.Domain.Models;

public class EnterpriseName
{
    private readonly string _value;

    private EnterpriseName(string value) => _value = value;

    public static Either<Error, EnterpriseName> Create(string? enterpriseName)
    {
        return string.IsNullOrWhiteSpace(enterpriseName)
            ? new EnterpriseName(string.Empty)
            : new EnterpriseName(enterpriseName);
    }

    public override string ToString() => _value;

    public static Either<Error, EnterpriseName> CreateUnsafe(string? givenEnterpriseName) => new EnterpriseName(givenEnterpriseName ?? string.Empty);
}