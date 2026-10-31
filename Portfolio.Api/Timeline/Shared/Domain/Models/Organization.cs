using LanguageExt;
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

    public static Either<DomainError, Organization> Create(string organization)
    {
        if (string.IsNullOrWhiteSpace(organization))
        {
            return new ValidationError("Organization cannot be empty");
        }

        var trimmed = organization.Trim();
        if (trimmed.Length > CharacterLimit)
        {
            return new ValidationError($"The organization cannot exceed {CharacterLimit} characters");
        }

        return new Organization(trimmed);
    }

    public static Either<DomainError, Organization> CreateUnsafe(string organization) => new Organization(organization);

    public override string ToString() => _organization;
}
