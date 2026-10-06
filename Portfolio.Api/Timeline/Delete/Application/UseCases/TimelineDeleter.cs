using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Timeline.Delete.Application.Dtos;
using Portfolio.Api.Timeline.Delete.Domain.Ports;
using Portfolio.Api.Timeline.Shared.Domain.Models;

namespace Portfolio.Api.Timeline.Delete.Application.UseCases;

public class TimelineDeleter(ITimelineDeleterRepository deleter)
{
    public async Task<Either<Error, Unit>> Execute(TimelineDeleterInfo timelineDeleterInfo)
    {
        return await (
            from id in TimelineId.Create(timelineDeleterInfo.Id).ToAsync()
            from result in deleter.DeleteEntry(id).ToAsync()
            select result
        );
    }
}
