using LanguageExt;
using Portfolio.Api.Timeline.Delete.Application.Dtos;
using Portfolio.Api.Timeline.Delete.Domain.Ports;
using Portfolio.Api.Timeline.Shared.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Timeline.Delete.Application.UseCases;

public class TimelineDeleter(ITimelineDeleterRepository deleter)
{
    public async Task<Either<DomainError, Unit>> Execute(TimelineDeleterInfo timelineDeleterInfo)
    {
        return await (
            from id in TimelineId.Create(timelineDeleterInfo.Id).ToAsync()
            from result in deleter.DeleteEntry(id).ToAsync()
            select result
        );
    }
}
