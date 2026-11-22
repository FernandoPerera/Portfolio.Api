namespace Portfolio.Api.WaysOfWorking.Create.Application.Dtos;

public record WayOfWorkingCreatorInfo(
    string Illustration,
    List<WayTranslationInfo> Translations
);
