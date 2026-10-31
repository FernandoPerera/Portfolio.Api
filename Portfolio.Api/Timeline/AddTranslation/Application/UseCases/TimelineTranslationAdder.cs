using LanguageExt;
using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Profiles.Shared.Domain.Ports;
using Portfolio.Api.Timeline.AddTranslation.Application.Dtos;
using Portfolio.Api.Timeline.AddTranslation.Domain.Models;
using Portfolio.Api.Timeline.AddTranslation.Domain.Ports;
using Portfolio.Api.Timeline.Shared.Domain.Models;
using Portfolio.Api.Timeline.Update.Domain.Ports;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Timeline.AddTranslation.Application.UseCases;

public class TimelineTranslationAdder(
    ITimelineEntryFinderRepository finder,
    ILanguageExistenceRepository languageExistence,
    ITimelineTranslationAdderRepository adder
)
{
    public async Task<Either<DomainError, Unit>> Execute(TimelineTranslationAdderInfo timelineTranslationAdderInfo)
    {
        return await (
            from id in TimelineId.Create(timelineTranslationAdderInfo.Id).ToAsync()
            from current in finder.FindEntry(id).ToAsync()
            from languageCode in LanguageCode.Create(timelineTranslationAdderInfo.LanguageCode).ToAsync()
            from _ in languageExistence.Exists(languageCode).ToAsync()
            from translation in TimelineTranslationToBeAdded.Create(
                current,
                timelineTranslationAdderInfo.LanguageCode,
                timelineTranslationAdderInfo.Title,
                timelineTranslationAdderInfo.Description
            ).ToAsync()
            from __ in adder.AddTranslation(translation).ToAsync()
            select Unit.Default
        );
    }
}
