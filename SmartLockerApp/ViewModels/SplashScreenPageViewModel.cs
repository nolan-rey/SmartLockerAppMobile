using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartLockerApp.Services;

namespace SmartLockerApp.ViewModels;

public partial class SplashScreenPageViewModel : BaseViewModel
{
    private readonly AppStateService _appState;

    [ObservableProperty]
    private string loadingText = "Initialisation...";

    [ObservableProperty]
    private bool isLoading = true;

    public SplashScreenPageViewModel(AppStateService appState)
    {
        _appState = appState;
        Title = "SmartLocker";
    }

    [RelayCommand]
    private async Task StartSplashSequence()
    {
        try
        {
            await SimulateAppLoading();
            await NavigateToMainApp();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur SplashScreen: {ex.Message}");
            await NavigateToMainApp();
        }
    }

    private async Task SimulateAppLoading()
    {
        var loadingTasks = new[]
        {
            "Initialisation des services...",
            "Chargement des données...",
            "Vérification de la connexion...",
            "Finalisation..."
        };

        foreach (var task in loadingTasks)
        {
            LoadingText = task;
            await Task.Delay(500);
        }

        LoadingText = "Prêt !";
        await Task.Delay(300);
    }

    private async Task NavigateToMainApp()
    {
        try
        {
            IsLoading = false;
            await Shell.Current.GoToAsync("//LoginPage");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur navigation: {ex.Message}");
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
