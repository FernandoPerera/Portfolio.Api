using LanguageExt;
using LanguageExt.Common;

namespace Portfolio.Api.Profiles.Create.Domain.Models;

public class Profile
{
    private readonly string _name;
    private readonly string _presentation;
    private readonly string _role;
    private readonly string _enterprise;

    private Profile(string name, string presentation, string role, string enterprise)
    {
        _name = name;
        _presentation = presentation;
        _role = role;
        _enterprise = enterprise;
    }

    public static Either<Error, Profile> Create(string name, string presentation, string role,
        string enterprise)
    {
        return new Profile(
            name, presentation, role, enterprise
        );
    }
}