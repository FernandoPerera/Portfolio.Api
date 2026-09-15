using LanguageExt;
using LanguageExt.Common;

namespace Portfolio.Api.Profiles.Create.Domain.Models;

public class JobRole
{
    private readonly string _value;

    private JobRole(string value) => _value = value;

    public static Either<Error, JobRole> Create(string jobRole)
    {
        return new JobRole(jobRole);
    }

    public override string ToString() => _value;
}