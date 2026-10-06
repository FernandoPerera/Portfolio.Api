using Portfolio.Api.Languages.Find.Application.UseCases;
using Portfolio.Api.Languages.Find.Domain.Ports;
using Portfolio.Api.Languages.Find.Infrastructure.Adapters;

namespace Portfolio.Api.Languages.Find.Infrastructure.Extensions;

public static class LanguageFindExtensions
{
    public static void AddLanguageFind(this IServiceCollection services)
    {
        services.AddScoped<LanguageFinder>();
        services.AddScoped<ILanguageFinderRepository, EntityFrameworkLanguageFinderAdapter>();
    }
}
