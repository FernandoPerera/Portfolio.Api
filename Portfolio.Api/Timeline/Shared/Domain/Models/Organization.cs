using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Timeline.Shared.Domain.Models;

public class Organization
{
    private const int CharacterLimit = 100;

    private readonly string _organization;

    private Organization(string organization)
    {
        _organization = organization;
    }

    public static Either<Error, Organization> Create(string organization)
    {
        if (string.IsNullOrWhiteSpace(organization))
        {
            return Error.New(new ValidationException("Organization cannot be empty"));
        }

        var trimmed = organization.Trim();
        if (trimmed.Length > CharacterLimit)
        {
            return Error.New(new ValidationException($"The organization cannot exceed {CharacterLimit} characters"));
        }

        return new Organization(trimmed);
    }

    public static Either<Error, Organization> CreateUnsafe(string organization) => new Organization(organization);

    public override string ToString() => _organization;
}
