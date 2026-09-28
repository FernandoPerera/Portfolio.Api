using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Api.Profiles.Create.Infrastructure.Entities;

[Table("Profiles")]
public class ProfileEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required] [MaxLength(100)] 
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)] 
    public string Enterprise { get; set; } = string.Empty;

    public ICollection<ProfileTranslationEntity> Translations { get; set; } = new List<ProfileTranslationEntity>();
}