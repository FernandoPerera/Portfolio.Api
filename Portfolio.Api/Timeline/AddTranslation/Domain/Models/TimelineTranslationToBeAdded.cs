using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.Timeline.Find.Domain.Models;
using Portfolio.Api.Timeline.Shared.Domain.Models;

namespace Portfolio.Api.Timeline.AddTranslation.Domain.Models;

public class TimelineTranslationToBeAdded
{
    private readonly Guid _entryId;
    private readonly LanguageCode _languageCode;
    private readonly TimelineTitle _title;
    private readonly TimelineDescription _description;

    private TimelineTranslationToBeAdded(
        Guid entryId,
        LanguageCode languageCode,
        TimelineTitle title,
        TimelineDescription description
    )
    {
        _entryId = entryId;
        _languageCode = languageCode;
        _title = title;
        _description = description;
    }

    public static Either<Error, TimelineTranslationToBeAdded> Create(
        TimelineEntryFound current,
        string givenLanguageCode,
        string givenTitle,
        string givenDescription
    ) =>
        from languageCode in LanguageCode.Create(givenLanguageCode)
        from _ in EnsureTranslationIsNew(current, languageCode)
        from title in TimelineTitle.Create(givenTitle)
        from description in TimelineDescription.Create(givenDescription)
        select new TimelineTranslationToBeAdded(current.GetId(), languageCode, title, description);

    public Guid GetEntryId() => _entryId;

    public string GetLanguageCode() => _languageCode.ToString();

    public LanguageCode GetLanguageCodeModel() => _languageCode;

    public string GetTitle() => _title.ToString();

    public string GetDescription() => _description.ToString();

    private static Either<Error, Unit> EnsureTranslationIsNew(TimelineEntryFound current, LanguageCode languageCode) =>
        current.GetTranslation(languageCode).Match<Either<Error, Unit>>(
            _ => Error.New(new ConflictException($"Timeline entry already has a translation for language '{languageCode}'")),
            _ => Unit.Default
        );
}
