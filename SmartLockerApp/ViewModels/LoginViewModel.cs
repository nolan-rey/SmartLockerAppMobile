using SmartLockerApp.Interfaces;
using SmartLockerApp.Models;
using System.Windows.Input;

namespace SmartLockerApp.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private readonly IDataService _dataService;
    private string _email = string.Empty;
    private string _password = string.Empty;
    private bool _isPasswordVisible = false;

    public LoginViewModel(IDataService dataService)
    {
        _dataService = dataService;
        Title = "Connexion";

        // Commands
        LoginCommand = new AsyncRelayCommand(LoginAsync, CanLogin);
        TogglePasswordVisibilityCommand = new RelayCommand(TogglePasswordVisibility);
        NavigateToSignupCommand = new AsyncRelayCommand(NavigateToSignupAsync);
        ForgotPasswordCommand = new AsyncRelayCommand(ForgotPasswordAsync);
    }

    // Properties
    public string Email
    {
        get => _email;
        set
        {
            if (SetProperty(ref _email, value))
            {
                ((AsyncRelayCommand)LoginCommand).RaiseCanExecuteChanged();
            }
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            if (SetProperty(ref _password, value))
            {
                ((AsyncRelayCommand)LoginCommand).RaiseCanExecuteChanged();
            }
        }
    }

    public bool IsPasswordVisible
    {
        get => _isPasswordVisible;
        set => SetProperty(ref _isPasswordVisible, value);
    }

    public string PasswordVisibilityIcon => IsPasswordVisible ? "👁️" : "👁️‍🗨️";

    // Commands
    public ICommand LoginCommand { get; }
    public ICommand TogglePasswordVisibilityCommand { get; }
    public ICommand NavigateToSignupCommand { get; }
    public ICommand ForgotPasswordCommand { get; }

    // Methods
    private bool CanLogin()
    {
        return !string.IsNullOrWhiteSpace(Email) && 
               !string.IsNullOrWhiteSpace(Password) && 
               !IsBusy;
    }

    private async Task LoginAsync()
    {
        var result = await _dataService.AuthenticateAsync(Email, Password);
        
        if (result.Success && result.User != null)
        {
            // Navigate to main page
            await Shell.Current.GoToAsync("//HomePage");
        }
        else
        {
            await Application.Current?.MainPage?.DisplayAlert(
                "Erreur de connexion", 
                result.Message ?? "Email ou mot de passe incorrect", 
                "OK");
        }
    }

    private void TogglePasswordVisibility()
    {
        IsPasswordVisible = !IsPasswordVisible;
        OnPropertyChanged(nameof(PasswordVisibilityIcon));
    }

    private async Task NavigateToSignupAsync()
    {
        await Shell.Current.GoToAsync("//SignupPage");
    }

    private async Task ForgotPasswordAsync()
    {
        await Application.Current?.MainPage?.DisplayAlert(
            "Mot de passe oublié", 
            "Fonctionnalité à implémenter avec l'API", 
            "OK");
    }
}
