using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using SmartLockerApp.Services;

namespace SmartLockerApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Configuration des services API
            ConfigureApiServices(builder.Services);

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        /// <summary>
        /// Configure les services API pour l'injection de dépendances
        /// </summary>
        private static void ConfigureApiServices(IServiceCollection services)
        {
            // Services API de base
            services.AddSingleton<ApiHttpService>();
            services.AddSingleton<SmartLockerApiService>();

            // Service intégré principal (recommandé pour les ViewModels)
            services.AddSingleton<SmartLockerIntegratedService>();

            // Services existants (si ils existent)
            // services.AddSingleton<AppStateService>();
            // services.AddSingleton<AuthenticationService>();
            // services.AddSingleton<LocalDataService>();
        }
    }
}
