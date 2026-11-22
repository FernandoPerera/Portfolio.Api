using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Api.WaysOfWorking.Shared.Infrastructure.Entities;

[Table("WaysOfWorking")]
public class WayOfWorkingEntity
{
    [Key]
    public Guid Id { get; set; }

    public int Position { get; set; }

    [Required] [MaxLength(30)]
    public string Illustration { get; set; } = string.Empty;

    public ICollection<WayOfWorkingTranslationEntity> Translations { get; set; } = new List<WayOfWorkingTranslationEntity>();
}
