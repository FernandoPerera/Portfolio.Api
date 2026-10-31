using LanguageExt;
using Portfolio.Api.Profiles.Shared.Domain.Ports;
using Portfolio.Api.Timeline.Create.Application.Dtos;
using Portfolio.Api.Timeline.Create.Domain.Models;
using Portfolio.Api.Timeline.Create.Domain.Ports;
using Portfolio.Api.Timeline.Shared.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Timeline.Create.Application.UseCases;

public class TimelineCreator(
    ITimelineCreatorRepository creator,
    ILanguageExistenceRepository languageExistence
)
{
    public async Task<Either<DomainError, Unit>> Execute(TimelineCreatorInfo timelineCreatorInfo)
    {
        return await (
            from translations in timelineCreatorInfo.Translations
                .Select(translation => TimelineTranslation.Create(
                    translation.LanguageCode,
                    translation.Title,
                    translation.Description
                ))
                .Sequence()
                .ToAsync()
            from entry in TimelineEntryToBeCreated.Create(
                timelineCreatorInfo.Organization,
                timelineCreatorInfo.Category,
                timelineCreatorInfo.StartDate,
                timelineCreatorInfo.EndDate,
                timelineCreatorInfo.Technologies,
                translations.ToList()
            ).ToAsync()
            from _ in EnsureLanguagesExist(entry).ToAsync()
            from result in creator.CreateEntry(entry).ToAsync()
            select result
        );
    }

    private async Task<Either<DomainError, Unit>> EnsureLanguagesExist(TimelineEntryToBeCreated entry)
    {
        foreach (var translation in entry.GetTranslations())
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
