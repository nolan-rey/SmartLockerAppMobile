using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartLockerApp.Services;

namespace SmartLockerApp.ViewModels;

/// <summary>
/// ViewModel pour la page des paramètres
/// </summary>
public partial class SettingsPageViewModel : BaseViewModel
{
    private readonly AppStateService _appState;
    private readonly AuthenticationService _authService;

    #region Observable Properties

    [ObservableProperty]
    private string userName = "Utilisateur";

    [ObservableProperty]
    private string userEmail = "email@example.com";

    #endregion

    public SettingsPageViewModel(AppStateService appState, AuthenticationService authService)
    {
        _authService = authService;
        _appState = appState;
        Title = "Paramètres";
        LoadUserInfo();
    }

    #region Commands

    [RelayCommand]
    private async Task NavigateBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task EditProfile()
    {
        await Shell.Current.DisplayAlert("Profil", "Modification du profil à implémenter", "OK");
    }

    [RelayCommand]
    private async Task ChangePassword()
    {
        await Shell.Current.DisplayAlert("Mot de passe", "Changement de mot de passe à implémenter", "OK");
    }

    [RelayCommand]
    private async Task ShowHelp()
    {
        await Shell.Current.GoToAsync("//HelpTutorialPage");
    }

    [RelayCommand]
    private async Task ContactSupport()
    {
        await Shell.Current.DisplayAlert("Support", "Contacter le support à implémenter", "OK");
    }

    [RelayCommand]
    private async Task ShowAbout()
    {
        await Shell.Current.DisplayAlert("À propos", "SmartLocker App v1.0\n\nApplication de gestion de casiers intelligents", "OK");
    }

    [RelayCommand]
    private async Task Logout()
    {
        bool result = await Shell.Current.DisplayAlert("Déconnexion", "Êtes-vous sûr de vouloir vous déconnecter?", "Oui", "Non");
        if (result)
        {
            // Déconnexion de l'utilisateur via le service d'authentification
            await _authService.LogoutAsync();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }

    #endregion

    #region Private Methods

    private void LoadUserInfo()
    {
        if (_appState.CurrentUser != null)
        {
            UserName = _appState.CurrentUser.name;
            UserEmail = _appState.CurrentUser.email;
        }
    }

    #endregion
}
