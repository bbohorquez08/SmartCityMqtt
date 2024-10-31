using Carter;
using SmartCity.Infrastructure.Services.MqttServices;

namespace SmartCity.Infrastructure.DependencyInjection
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddInfrastructureServices
        (this IServiceCollection services)
        {
            services.AddHostedService<MqttService>();
            services.AddSingleton<MqttService>();
            services.AddCarter();
            return services;
        }

    }
}
