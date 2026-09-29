using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Profiles.Create.Domain.Models;

public class JobRole
{
    private readonly string _value;

    private JobRole(string value) => _value = value;

    public static Either<Error, JobRole> Create(string jobRole)
    {
        if (string.IsNullOrWhiteSpace(jobRole))
        {
            return Error.New(new ValidationException("Job role cannot be empty"));
        }
        
        return new JobRole(jobRole);
    }

    public override string ToString() => _value;
}