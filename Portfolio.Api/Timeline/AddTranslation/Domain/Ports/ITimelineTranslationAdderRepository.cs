using LanguageExt;
using Portfolio.Api.Timeline.AddTranslation.Domain.Models;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Timeline.AddTranslation.Domain.Ports;

public interface ITimelineTranslationAdderRepository
{
    Task<Either<DomainError, Unit>> AddTranslation(TimelineTranslationToBeAdded translationToBeAdded);
}
