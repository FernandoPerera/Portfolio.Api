using LanguageExt;
using Portfolio.Api.Timeline.Find.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Timeline.Find.Domain.Ports;

public interface ITimelineFinderRepository
{
    Task<Either<DomainError, List<TimelineEntryFound>>> FindEntries();
}
