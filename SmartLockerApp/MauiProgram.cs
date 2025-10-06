using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using SmartLockerApp.Interfaces;
using SmartLockerApp.Services;
using SmartLockerApp.ViewModels;
using SmartLockerApp.Views;

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

            // Services - ordre important pour l'injection de dépendances
            builder.Services.AddSingleton<LocalStorageService>();
            
            // API Services (doivent être avant les services qui les utilisent)
            builder.Services.AddSingleton<ApiAuthService>();
            builder.Services.AddSingleton<ApiHttpClient>();
            builder.Services.AddSingleton<ApiUserService>();
            builder.Services.AddSingleton<ApiLockerService>();
            builder.Services.AddSingleton<ApiSessionService>();
            builder.Services.AddSingleton<ApiAuthMethodService>();
            builder.Services.AddSingleton<ApiSessionAuthService>();
            
            // Services locaux
            builder.Services.AddSingleton<AuthenticationService>();
            builder.Services.AddSingleton<LockerManagementService>();
            builder.Services.AddSingleton<UserService>();
            builder.Services.AddSingleton<AppStateService>();
            
            // Data Service - Utiliser ApiDataService pour l'API ou LocalDataService pour local
            // Décommenter la ligne souhaitée :
            builder.Services.AddSingleton<IDataService, ApiDataService>();  // ✅ Utilise l'API SmartLocker
            // builder.Services.AddSingleton<IDataService, LocalDataService>();  // Utilise les données locales
            
            builder.Services.AddSingleton<ViewModelLocator>();

            // ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<DepositSetupPageViewModel>();
            builder.Services.AddTransient<PaymentPageViewModel>();
            builder.Services.AddTransient<ActiveSessionPageViewModel>();
            builder.Services.AddTransient<HistoryPageViewModel>();
            builder.Services.AddTransient<LockerDetailPageViewModel>();
            builder.Services.AddTransient<SettingsPageViewModel>();
            builder.Services.AddTransient<LockerOpenedPageViewModel>();
            builder.Services.AddTransient<UnlockInstructionsPageViewModel>();
            builder.Services.AddTransient<SplashScreenPageViewModel>();

            // Pages
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<SignupPage>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<DepositSetupPage>();
            builder.Services.AddTransient<PaymentPage>();
            builder.Services.AddTransient<ActiveSessionPage>();
            builder.Services.AddTransient<HistoryPage>();
            builder.Services.AddTransient<LockerDetailPage>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<LockerOpenedPage>();
            builder.Services.AddTransient<UnlockInstructionsPage>();
            builder.Services.AddTransient<SplashScreenPage>();
            builder.Services.AddTransient<LockInstructionsPage>();
            builder.Services.AddTransient<LockConfirmationPage>();
            builder.Services.AddTransient<DepositItemsPage>();
            builder.Services.AddTransient<OpenLockerPage>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
