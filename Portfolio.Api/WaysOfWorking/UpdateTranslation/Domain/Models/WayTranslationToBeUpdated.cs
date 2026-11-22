using LanguageExt;
using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;
using Portfolio.Api.WaysOfWorking.Find.Domain.Models;
using Portfolio.Api.WaysOfWorking.Shared.Domain.Models;
using static LanguageExt.Prelude;

namespace Portfolio.Api.WaysOfWorking.UpdateTranslation.Domain.Models;

public class WayTranslationToBeUpdated
{
    private readonly WayOfWorkingId _wayId;
    private readonly LanguageCode _languageCode;
    private readonly WayLabel _label;
    private readonly WayCaption _caption;
    private readonly WayTitle _title;
    private readonly WayDescription _description;
    private readonly WayPoints _points;

    private WayTranslationToBeUpdated(
        WayOfWorkingId wayId,
        LanguageCode languageCode,
        WayLabel label,
        WayCaption caption,
        WayTitle title,
        WayDescription description,
        WayPoints points
    )
    {
        _wayId = wayId;
        _languageCode = languageCode;
        _label = label;
        _caption = caption;
        _title = title;
        _description = description;
        _points = points;
    }

    public static Either<DomainError, WayTranslationToBeUpdated> Create(
        WayOfWorkingFound current,
        string givenLanguageCode,
        string? givenLabel,
        string? givenCaption,
        string? givenTitle,
        string? givenDescription,
        List<string>? givenPoints
    ) =>
        from languageCode in LanguageCode.Create(givenLanguageCode)
        from translation in current.GetTranslation(languageCode)
        from wayId in WayOfWorkingId.Create(current.GetId())
        from label in Optional(givenLabel)
            .Match(WayLabel.Create, WayLabel.CreateUnsafe(translation.GetLabel()))
        from caption in Optional(givenCaption)
            .Match(WayCaption.Create, WayCaption.CreateUnsafe(translation.GetCaption()))
        from title in Optional(givenTitle)
            .Match(WayTitle.Create, WayTitle.CreateUnsafe(translation.GetTitle()))
        from description in Optional(givenDescription)
            .Match(WayDescription.Create, WayDescription.CreateUnsafe(translation.GetDescription()))
        from points in Optional(givenPoints)
            .Match(WayPoints.Create, WayPoints.CreateUnsafe(translation.GetPoints()))
        select new WayTranslationToBeUpdated(wayId, languageCode, label, caption, title, description, points);

    public Guid GetWayId() => _wayId.ToGuid();

    public string GetLanguageCode() => _languageCode.ToString();

    public string GetLabel() => _label.ToString();

    public string GetCaption() => _caption.ToString();

    public string GetTitle() => _title.ToString();

    public string GetDescription() => _description.ToString();

    public List<string> GetPoints() => _points.GetValues();
}
