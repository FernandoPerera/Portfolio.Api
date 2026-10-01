using Portfolio.Api.Languages.Create.Application.UseCases;
using Portfolio.Api.Languages.Create.Domain.Ports;
using Portfolio.Api.Languages.Create.Infrastructure.Adapters;

namespace Portfolio.Api.Languages.Create.Infrastructure.Extensions;

public static class LanguageCreateExtensions
{
    public static void AddLanguageCreate(this IServiceCollection services)
    {
        services.AddScoped<LanguageCreator>();
        services.AddScoped<ILanguageCreatorRepository, EntityFrameworkLanguageCreatorAdapter>();
    }
}
