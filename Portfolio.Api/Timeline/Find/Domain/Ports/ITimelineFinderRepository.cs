using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Timeline.Find.Domain.Models;

namespace Portfolio.Api.Timeline.Find.Domain.Ports;

public interface ITimelineFinderRepository
{
    Task<Either<Error, List<TimelineEntryFound>>> FindEntries();
}
