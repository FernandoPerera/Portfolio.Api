using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Timeline.Shared.Domain.Models;
using Portfolio.Api.Timeline.Update.Application.Dtos;
using Portfolio.Api.Timeline.Update.Domain.Models;
using Portfolio.Api.Timeline.Update.Domain.Ports;

namespace Portfolio.Api.Timeline.Update.Application.UseCases;

public class TimelineUpdater(
    ITimelineEntryFinderRepository finder,
    ITimelineUpdaterRepository updater
)
{
    public async Task<Either<Error, Unit>> Execute(TimelineUpdaterInfo timelineUpdaterInfo)
    {
        return await (
            from id in TimelineId.Create(timelineUpdaterInfo.Id).ToAsync()
            from current in finder.FindEntry(id).ToAsync()
            from entry in TimelineEntryToBeUpdated.Create(
                current,
                timelineUpdaterInfo.Organization,
                timelineUpdaterInfo.Category,
                timelineUpdaterInfo.StartDate,
                timelineUpdaterInfo.EndDate,
                timelineUpdaterInfo.Technologies,
                timelineUpdaterInfo.Title,
                timelineUpdaterInfo.Description,
                timelineUpdaterInfo.LanguageCode
            ).ToAsync()
            from result in updater.UpdateEntry(entry).ToAsync()
            select result
        );
    }
}
