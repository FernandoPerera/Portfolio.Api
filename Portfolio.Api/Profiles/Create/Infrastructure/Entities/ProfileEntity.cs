using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Api.Profiles.Create.Infrastructure.Entities;

[Table("Profiles")]
public class ProfileEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required] [MaxLength(100)] public string Name { get; set; } = string.Empty;

    [MaxLength(750)] public string Presentation { get; set; } = string.Empty;

    [MaxLength(50)] public string Role { get; set; } = string.Empty;

    [MaxLength(100)] public string Enterprise { get; set; } = string.Empty;
}