using System.ComponentModel.DataAnnotations;

namespace Portfolio.Api.Tests.Shared.Extensions;

public static class ValidationExtensions
{
    public static List<string> GetInvalidMembers(this object options)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(options, new ValidationContext(options), results, validateAllProperties: true);

        return results.SelectMany(result => result.MemberNames).ToList();
    }
}
