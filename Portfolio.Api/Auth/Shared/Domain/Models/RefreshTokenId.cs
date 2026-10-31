using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Auth.Shared.Domain.Models;

public class RefreshTokenId
{
    private readonly Guid _id;

    private RefreshTokenId(Guid id)
    {
        _id = id;
    }

    public static Either<DomainError, RefreshTokenId> Create(Guid id)
    {
        if (id == Guid.Empty)
        {
            return new ValidationError("Refresh token id cannot be empty");
        }

        return new RefreshTokenId(id);
    }

    public static Either<DomainError, RefreshTokenId> CreateUnsafe(Guid id) => new RefreshTokenId(id);

    public Guid GetValue() => _id;
}
