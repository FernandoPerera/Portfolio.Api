using Portfolio.Api.Languages.Delete.Application.UseCases;
using Portfolio.Api.Languages.Delete.Domain.Ports;
using Portfolio.Api.Languages.Delete.Infrastructure.Adapters;

namespace Portfolio.Api.Languages.Delete.Infrastructure.Extensions;

public static class LanguageDeleteExtensions
{
    public static void AddLanguageDelete(this IServiceCollection services)
    {
        services.AddScoped<LanguageDeleter>();
        services.AddScoped<ILanguageUsageRepository, EntityFrameworkLanguageUsageAdapter>();
        services.AddScoped<ILanguageDeleterRepository, EntityFrameworkLanguageDeleterAdapter>();
    }
}
