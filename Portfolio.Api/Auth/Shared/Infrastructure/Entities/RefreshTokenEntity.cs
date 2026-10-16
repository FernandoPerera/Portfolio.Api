using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Api.Auth.Shared.Infrastructure.Entities;

[Table("RefreshTokens")]
public class RefreshTokenEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required] [MaxLength(64)]
    public string TokenHash { get; set; } = string.Empty;

    [Required] [MaxLength(100)]
    public string Owner { get; set; } = string.Empty;

    public DateTimeOffset ExpiresAt { get; set; }

    [ConcurrencyCheck]
    public DateTimeOffset? ConsumedAt { get; set; }
}
