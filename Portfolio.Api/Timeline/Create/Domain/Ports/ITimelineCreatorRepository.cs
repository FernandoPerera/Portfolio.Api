using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Timeline.Create.Domain.Models;

namespace Portfolio.Api.Timeline.Create.Domain.Ports;

public interface ITimelineCreatorRepository
{
    Task<Either<Error, Unit>> CreateEntry(TimelineEntryToBeCreated timelineEntryToBeCreated);
}
