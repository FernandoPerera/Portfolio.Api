using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Timeline.Update.Domain.Models;

namespace Portfolio.Api.Timeline.Update.Domain.Ports;

public interface ITimelineUpdaterRepository
{
    Task<Either<Error, Unit>> UpdateEntry(TimelineEntryToBeUpdated timelineEntryToBeUpdated);
}
