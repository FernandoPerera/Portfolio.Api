namespace Portfolio.Api.Auth.Shared.Infrastructure.Authorization;

public static class AdminAccess
{
    public const string PolicyName = "Admin";
    public const string RoleClaimType = "role";
    public const string RoleClaimValue = "admin";
}
