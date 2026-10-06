using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Profiles.Shared.Domain.Ports;
using Portfolio.Api.Timeline.Create.Application.Dtos;
using Portfolio.Api.Timeline.Shared.Domain.Models;
using Portfolio.Api.Timeline.Update.Application.Dtos;
using Portfolio.Api.Timeline.Update.Domain.Models;
using Portfolio.Api.Timeline.Update.Domain.Ports;

namespace Portfolio.Api.Timeline.Update.Application.UseCases;

public class TimelineUpdater(
    ITimelineEntryFinderRepository finder,
    ITimelineUpdaterRepository updater,
    ILanguageExistenceRepository languageExistence
)
{
    public async Task<Either<Error, Unit>> Execute(TimelineUpdaterInfo timelineUpdaterInfo)
    {
        return await (
            from id in TimelineId.Create(timelineUpdaterInfo.Id).ToAsync()
            from current in finder.FindEntry(id).ToAsync()
            from translations in CreateTranslations(timelineUpdaterInfo.Translations).ToAsync()
            from entry in TimelineEntryToBeUpdated.Create(
                current,
                timelineUpdaterInfo.Organization,
                timelineUpdaterInfo.Category,
                timelineUpdaterInfo.StartDate,
                timelineUpdaterInfo.EndDate,
                timelineUpdaterInfo.Technologies,
                translations.IfNoneUnsafe((List<TimelineTranslation>?)null)
            ).ToAsync()
            from _ in EnsureLanguagesExist(entry).ToAsync()
            from result in updater.UpdateEntry(entry).ToAsync()
            select result
        );
    }

    private static Either<Error, Option<List<TimelineTranslation>>> CreateTranslations(
        List<TimelineTranslationInfo>? translations
    )
    {
        if (translations is null)
        {
            return Option<List<TimelineTranslation>>.None;
        }

        return translations
            .Select(translation => TimelineTranslation.Create(
                translation.LanguageCode,
                translation.Title,
                translation.Description
            ))
            .Sequence()
            .Map(created => Option<List<TimelineTranslation>>.Some(created.ToList()));
    }

    private async Task<Either<Error, Unit>> EnsureLanguagesExist(TimelineEntryToBeUpdated entry)
    {
        foreach (var translation in entry.GetTranslations() ?? [])
        {
            var exists = await languageExistence.Exists(translation.GetLanguageCodeModel());
            if (exists.IsLeft)
            {
                return exists;
            }
        }

        return Unit.Default;
    }
}
