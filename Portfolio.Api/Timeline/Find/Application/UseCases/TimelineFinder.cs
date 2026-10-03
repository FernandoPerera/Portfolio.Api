using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Timeline.Find.Domain.Models;
using Portfolio.Api.Timeline.Find.Domain.Ports;

namespace Portfolio.Api.Timeline.Find.Application.UseCases;

public class TimelineFinder(ITimelineFinderRepository repository)
{
    public async Task<Either<Error, List<TimelineEntryFound>>> Execute()
    {
        return await (
            from entries in repository.FindEntries().ToAsync()
            select entries
        );
    }
}
