using System.Globalization;
using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Auth.Shared.Domain.Models;

public class ExternalUserId
{
    private readonly string _id;

    private ExternalUserId(string id)
    {
        _id = id;
    }

    public static Either<DomainError, ExternalUserId> Create(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return new ValidationError("External user id cannot be empty");
        }

        return new ExternalUserId(id.Trim());
    }

    public static Either<DomainError, ExternalUserId> Create(long id)
    {
        if (id <= 0)
        {
            return new ValidationError("External user id must be positive");
        }

        return new ExternalUserId(id.ToString(CultureInfo.InvariantCulture));
    }

    public override string ToString() => _id;
}
