using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Profiles.Find.Domain.Models;

public class EnterpriseName
{
    private readonly string _value;

    private EnterpriseName(string value) => _value = value;

    public static Either<DomainError, EnterpriseName> Create(string? enterpriseName)
    {
        return string.IsNullOrWhiteSpace(enterpriseName)
            ? new EnterpriseName(string.Empty)
            : new EnterpriseName(enterpriseName);
    }

    public override string ToString() => _value;

    public static Either<DomainError, EnterpriseName> CreateUnsafe(string? givenEnterpriseName) => new EnterpriseName(givenEnterpriseName ?? string.Empty);
}
