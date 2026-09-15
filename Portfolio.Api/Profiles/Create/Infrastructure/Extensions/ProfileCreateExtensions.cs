using Portfolio.Api.Profiles.Create.Application.UseCases;
using Portfolio.Api.Profiles.Create.Domain.Ports;
using Portfolio.Api.Profiles.Create.Infrastructure.Adapters;

namespace Portfolio.Api.Profiles.Create.Infrastructure.Extensions;

public static class ProfileCreateExtensions
{
    public static void AddProfileCreate(this IServiceCollection services)
    {
        services.AddScoped<IProfileCreatorRepository, EntityFrameworkProfileCreatorAdapter>();
        services.AddScoped<ProfileCreator>();
    }
}
