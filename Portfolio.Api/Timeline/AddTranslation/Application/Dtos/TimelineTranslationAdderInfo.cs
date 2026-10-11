namespace Portfolio.Api.Timeline.AddTranslation.Application.Dtos;

public record TimelineTranslationAdderInfo(
    Guid Id,
    string LanguageCode,
    string Title,
    string Description
);
