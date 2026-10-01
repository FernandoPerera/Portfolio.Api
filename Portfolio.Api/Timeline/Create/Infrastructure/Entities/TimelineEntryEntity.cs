using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Api.Timeline.Create.Infrastructure.Entities;

[Table("TimelineEntries")]
public class TimelineEntryEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required] [MaxLength(100)]
    public string Organization { get; set; } = string.Empty;

    [Required] [MaxLength(20)]
    public string Category { get; set; } = string.Empty;

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public List<string> Technologies { get; set; } = [];

    public ICollection<TimelineEntryTranslationEntity> Translations { get; set; } = new List<TimelineEntryTranslationEntity>();
}
