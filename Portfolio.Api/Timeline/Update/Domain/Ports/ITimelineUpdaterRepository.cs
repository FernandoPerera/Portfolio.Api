using LanguageExt;
using Portfolio.Api.Timeline.Update.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Timeline.Update.Domain.Ports;

public interface ITimelineUpdaterRepository
{
    Task<Either<DomainError, Unit>> UpdateEntry(TimelineEntryToBeUpdated timelineEntryToBeUpdated);
}
