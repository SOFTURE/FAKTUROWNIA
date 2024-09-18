using Microsoft.Extensions.DependencyInjection;
using Refit;
using SOFTURE.Common.HealthCheck;
using SOFTURE.Fakturownia.Abstractions;
using SOFTURE.Fakturownia.Delegates;
using SOFTURE.Fakturownia.HealthChecks;
using SOFTURE.Fakturownia.Settings;
using SOFTURE.Settings.Extensions;

namespace SOFTURE.Fakturownia
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddFakturownia<TSettings>(this IServiceCollection services)
            where TSettings : IFakturowniaSettings
        {
            var settings = services.GetSettings<TSettings, FakturowniaSettings>(x => x.Fakturownia);

            services.AddCommonHealthCheck<FakturowniaHealthCheck>();
            
            services.AddTransient<GetApiTokenHandler>();

            services
                .AddRefitClient<IFakturowniaApi>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(settings.Url))
                .AddHttpMessageHandler<GetApiTokenHandler>();
            
            services.AddScoped<IFakturowniaClient, FakturowniaClient>();    
            
            return services;
        }
    }
}