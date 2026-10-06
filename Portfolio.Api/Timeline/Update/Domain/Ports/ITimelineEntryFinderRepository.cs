using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Timeline.Find.Domain.Models;

namespace Portfolio.Api.Timeline.Update.Domain.Ports;

public interface ITimelineEntryFinderRepository
{
    Task<Either<Error, TimelineEntryFound>> FindEntry(Guid id);
}
