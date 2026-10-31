using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Profiles.Find.Domain.Models;

public class JobRole
{
    private readonly string _value;

    private JobRole(string value) => _value = value;

    public static Either<DomainError, JobRole> Create(string jobRole)
    {
        if (string.IsNullOrWhiteSpace(jobRole))
        {
            return new ValidationError("Job role cannot be empty");
        }
        
        return new JobRole(jobRole);
    }

    public override string ToString() => _value;

    public static Either<DomainError, JobRole> CreateUnsafe(string givenJobRole) => new JobRole(givenJobRole);
}
