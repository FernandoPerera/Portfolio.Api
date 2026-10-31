using LanguageExt;
using Portfolio.Api.Timeline.Shared.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Timeline.Delete.Domain.Ports;

public interface ITimelineDeleterRepository
{
    Task<Either<DomainError, Unit>> DeleteEntry(TimelineId id);
}
