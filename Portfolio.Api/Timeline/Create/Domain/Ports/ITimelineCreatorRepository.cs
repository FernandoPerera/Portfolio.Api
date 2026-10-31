using LanguageExt;
using Portfolio.Api.Timeline.Create.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Timeline.Create.Domain.Ports;

public interface ITimelineCreatorRepository
{
    Task<Either<DomainError, Unit>> CreateEntry(TimelineEntryToBeCreated timelineEntryToBeCreated);
}
