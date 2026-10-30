using Portfolio.Api.Auth.Logout.Application.Dtos;

namespace Portfolio.Api.Tests.Auth.Logout.Application.Builders;

public abstract class LogoutInfoBuilder
{
    public static LogoutInfo Build(string refreshToken = "current-refresh-token")
    {
        return new LogoutInfo(refreshToken);
    }
}
