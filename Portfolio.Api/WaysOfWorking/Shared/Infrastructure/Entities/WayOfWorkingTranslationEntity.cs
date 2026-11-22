using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Profiles.Shared.Infrastructure.Entities;

namespace Portfolio.Api.WaysOfWorking.Shared.Infrastructure.Entities;

[Table("WayOfWorkingTranslations")]
[PrimaryKey(nameof(WayId), nameof(LanguageCode))]
public class WayOfWorkingTranslationEntity
{
    public Guid WayId { get; set; }
    public WayOfWorkingEntity Way { get; set; } = null!;

    [MaxLength(2)]
    public string LanguageCode { get; set; } = string.Empty;

    public LanguageEntity Language { get; set; } = null!;

    [MaxLength(30)]
    public string Label { get; set; } = string.Empty;

    [MaxLength(60)]
    public string Caption { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public List<string> Points { get; set; } = [];
}
