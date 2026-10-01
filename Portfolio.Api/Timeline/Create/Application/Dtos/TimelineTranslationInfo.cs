namespace Portfolio.Api.Timeline.Create.Application.Dtos;

public record TimelineTranslationInfo(
    string LanguageCode,
    string Title,
    string Description
);
