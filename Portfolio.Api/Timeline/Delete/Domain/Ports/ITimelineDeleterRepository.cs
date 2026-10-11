using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Timeline.Shared.Domain.Models;

namespace Portfolio.Api.Timeline.Delete.Domain.Ports;

public interface ITimelineDeleterRepository
{
    Task<Either<Error, Unit>> DeleteEntry(TimelineId id);
}
