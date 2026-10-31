using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Profiles.Shared.Domain.Models;

public class Presentation
{
    private const int CharacterLimit = 750;
    
    private readonly string _value;

    private Presentation(string value) => _value = value;

    public static Either<DomainError, Presentation> Create(string presentation)
    {
        if (string.IsNullOrWhiteSpace(presentation))
        {
            return new ValidationError("Presentation cannot be empty");
        }
        
        if (presentation.Length > CharacterLimit)
        {
            return new ValidationError($"The presentation cannot exceed {CharacterLimit} characters");
        }
        
        return new Presentation(presentation);
    }

    public override string ToString() => _value;

    public static Either<DomainError, Presentation> CreateUnsafe(string givenPresentation) => new Presentation(givenPresentation);
}