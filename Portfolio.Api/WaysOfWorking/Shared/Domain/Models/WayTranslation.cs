using LanguageExt;
using Portfolio.Api.Profiles.Shared.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.WaysOfWorking.Shared.Domain.Models;

public class WayTranslation
{
    private readonly LanguageCode _languageCode;
    private readonly WayLabel _label;
    private readonly WayCaption _caption;
    private readonly WayTitle _title;
    private readonly WayDescription _description;
    private readonly WayPoints _points;

    private WayTranslation(
        LanguageCode languageCode,
        WayLabel label,
        WayCaption caption,
        WayTitle title,
        WayDescription description,
        WayPoints points
    )
    {
        _languageCode = languageCode;
        _label = label;
        _caption = caption;
        _title = title;
        _description = description;
        _points = points;
    }

    public static Either<DomainError, WayTranslation> Create(
        string givenLanguageCode,
        string givenLabel,
        string givenCaption,
        string givenTitle,
        string givenDescription,
        List<string> givenPoints
    ) =>
        from languageCode in LanguageCode.Create(givenLanguageCode)
        from label in WayLabel.Create(givenLabel)
        from caption in WayCaption.Create(givenCaption)
        from title in WayTitle.Create(givenTitle)
        from description in WayDescription.Create(givenDescription)
        from points in WayPoints.Create(givenPoints)
        select new WayTranslation(languageCode, label, caption, title, description, points);

    public static Either<DomainError, WayTranslation> CreateUnsafe(
        string givenLanguageCode,
        string givenLabel,
        string givenCaption,
        string givenTitle,
        string givenDescription,
        List<string> givenPoints
    ) =>
        from languageCode in LanguageCode.CreateUnsafe(givenLanguageCode)
        from label in WayLabel.CreateUnsafe(givenLabel)
        from caption in WayCaption.CreateUnsafe(givenCaption)
        from title in WayTitle.CreateUnsafe(givenTitle)
        from description in WayDescription.CreateUnsafe(givenDescription)
        from points in WayPoints.CreateUnsafe(givenPoints)
        select new WayTranslation(languageCode, label, caption, title, description, points);

    public string GetLanguageCode() => _languageCode.ToString();

    public LanguageCode GetLanguageCodeModel() => _languageCode;

    public string GetLabel() => _label.ToString();

    public string GetCaption() => _caption.ToString();

    public string GetTitle() => _title.ToString();

    public string GetDescription() => _description.ToString();

    public List<string> GetPoints() => _points.GetValues();
}
