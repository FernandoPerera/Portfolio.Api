using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Api.Profiles.Shared.Infrastructure.Entities;

[Table("Languages")]
public class LanguageEntity
{
    [Key] [MaxLength(2)] 
    public string LanguageCode { get; set; } = string.Empty;

    [MaxLength(20)] 
    public string Name { get; set; } = string.Empty;
}
