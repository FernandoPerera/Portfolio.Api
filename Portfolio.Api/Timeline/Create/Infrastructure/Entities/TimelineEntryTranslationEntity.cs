using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Profiles.Shared.Infrastructure.Entities;

namespace Portfolio.Api.Timeline.Create.Infrastructure.Entities;

[Table("TimelineEntryTranslations")]
[PrimaryKey(nameof(EntryId), nameof(LanguageCode))]
public class TimelineEntryTranslationEntity
{
    public Guid EntryId { get; set; }
    public TimelineEntryEntity Entry { get; set; } = null!;

    [MaxLength(2)]
    public string LanguageCode { get; set; } = string.Empty;

    public LanguageEntity Language { get; set; } = null!;

    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
}
