using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Timeline.Shared.Domain.Models;

public class Technologies
{
    private readonly List<Technology> _technologies;

    private Technologies(List<Technology> technologies)
    {
        _technologies = technologies;
    }

    public static Either<DomainError, Technologies> Create(List<string> givenTechnologies)
    {
        var hasRepeatedTechnologies = givenTechnologies
            .GroupBy(technology => technology.Trim().ToLowerInvariant())
            .Any(group => group.Count() > 1);
        if (hasRepeatedTechnologies)
        {
            return new ValidationError("Technologies cannot be repeated");
        }

        return givenTechnologies
            .Select(Technology.Create)
            .Sequence()
            .Map(technologies => new Technologies(technologies.ToList()));
    }

    public static Either<DomainError, Technologies> CreateUnsafe(List<string> givenTechnologies) =>
        givenTechnologies
            .Select(Technology.CreateUnsafe)
            .Sequence()
            .Map(technologies => new Technologies(technologies.ToList()));

    public List<string> GetValues() => _technologies.Select(technology => technology.ToString()).ToList();
}
