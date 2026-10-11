using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Timeline.AddTranslation.Domain.Models;

namespace Portfolio.Api.Timeline.AddTranslation.Domain.Ports;

public interface ITimelineTranslationAdderRepository
{
    Task<Either<Error, Unit>> AddTranslation(TimelineTranslationToBeAdded translationToBeAdded);
}
