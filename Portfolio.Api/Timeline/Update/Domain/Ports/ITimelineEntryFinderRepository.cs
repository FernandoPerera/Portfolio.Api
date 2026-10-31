using LanguageExt;
using Portfolio.Api.Timeline.Find.Domain.Models;
using Portfolio.Api.Timeline.Shared.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Timeline.Update.Domain.Ports;

public interface ITimelineEntryFinderRepository
{
    Task<Either<DomainError, TimelineEntryFound>> FindEntry(TimelineId id);
}
