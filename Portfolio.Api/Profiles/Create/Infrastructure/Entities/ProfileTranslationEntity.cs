using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Profiles.Shared.Infrastructure.Entities;

namespace Portfolio.Api.Profiles.Create.Infrastructure.Entities;

[Table("ProfileTranslations")]
[PrimaryKey(nameof(ProfileId), nameof(LanguageCode))]
public class ProfileTranslationEntity
{
    public Guid ProfileId { get; set; }
    public ProfileEntity Profile { get; set; } = null!;

    [MaxLength(2)] 
    public string LanguageCode { get; set; } = string.Empty;

    public LanguageEntity Language { get; set; } = null!;

    [MaxLength(750)] 
    public string Presentation { get; set; } = string.Empty;

    [MaxLength(50)] 
    public string Role { get; set; } = string.Empty;
}
