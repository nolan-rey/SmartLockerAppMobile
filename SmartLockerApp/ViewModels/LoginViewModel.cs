using SmartLockerApp.Interfaces;
using SmartLockerApp.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SmartLockerApp.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IDataService _dataService;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string email = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string password = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PasswordVisibilityIcon))]
    private bool isPasswordVisible = false;

    public LoginViewModel(IDataService dataService)
    {
        _dataService = dataService;
        Title = "Connexion";
    }

    public string PasswordVisibilityIcon => IsPasswordVisible ? "👁️" : "👁️‍🗨️";

    // Commands avec RelayCommand
    [RelayCommand(CanExecute = nameof(CanLogin))]
    private async Task Login()
    {
        var result = await _dataService.AuthenticateAsync(Email, Password);
        
        if (result.Success && result.User != null)
        {
            // Navigate to main page
            await Shell.Current.GoToAsync("//HomePage");
        }
        else
        {
            if (Application.Current?.Windows?.Count > 0)
            {
                var mainPage = Application.Current.Windows[0].Page;
                if (mainPage != null)
                    await mainPage.DisplayAlert(
                        "Erreur de connexion", 
                        result.Message ?? "Email ou mot de passe incorrect", 
                        "OK");
            }
        }
    }

    [RelayCommand]
    private void TogglePasswordVisibility()
    {
        IsPasswordVisible = !IsPasswordVisible;
    }

    [RelayCommand]
    private async Task NavigateToSignup()
    {
        await Shell.Current.GoToAsync("//SignupPage");
    }

    [RelayCommand]
    private async Task ForgotPassword()
    {
        if (Application.Current?.Windows?.Count > 0)
        {
            var mainPage = Application.Current.Windows[0].Page;
            if (mainPage != null)
                await mainPage.DisplayAlert(
                    "Mot de passe oublié", 
                    "Fonctionnalité à venir", 
                    "OK");
        }
    }

    // Méthode CanExecute pour LoginCommand
    private bool CanLogin()
    {
        return !string.IsNullOrWhiteSpace(Email) && 
               !string.IsNullOrWhiteSpace(Password) && 
               !IsBusy;
    }
}
