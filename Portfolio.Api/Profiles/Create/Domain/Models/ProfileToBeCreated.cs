using LanguageExt;
using LanguageExt.Common;

namespace Portfolio.Api.Profiles.Create.Domain.Models;

public class ProfileToBeCreated
{
    private readonly string _name;
    private readonly string _presentation;
    private readonly string _role;
    private readonly string _enterprise;

    private ProfileToBeCreated(string name, string presentation, string role, string enterprise)
    {
        _name = name;
        _presentation = presentation;
        _role = role;
        _enterprise = enterprise;
    }

    public static Either<Error, ProfileToBeCreated> Create(string name, string presentation, string role,
        string enterprise)
    {
        return new ProfileToBeCreated(
            name, presentation, role, enterprise
        );
    }
}