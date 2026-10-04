using Portfolio.Api.Auth.Refresh.Application.Dtos;

namespace Portfolio.Api.Tests.Auth.Refresh.Application.Builders;

public abstract class RefreshInfoBuilder
{
    public static RefreshInfo Build(string refreshToken = "current-refresh-token")
    {
        return new RefreshInfo(refreshToken);
    }
}
