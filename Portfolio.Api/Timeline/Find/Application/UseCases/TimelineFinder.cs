using LanguageExt;
using Portfolio.Api.Timeline.Find.Domain.Models;
using Portfolio.Api.Timeline.Find.Domain.Ports;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Timeline.Find.Application.UseCases;

public class TimelineFinder(ITimelineFinderRepository repository)
{
    public async Task<Either<DomainError, List<TimelineEntryFound>>> Execute()
    {
        return await (
            from entries in repository.FindEntries().ToAsync()
            select entries
        );
    }
}
