using SmartLockerApp.Services;

namespace SmartLockerApp.Views;

public partial class LoginPage : ContentPage
{
    private readonly AppStateService _appState = AppStateService.Instance;

    public LoginPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await AnimationService.FadeInAsync(this);
        await AnimationService.SlideInFromBottomAsync(LoginCard, 400);
    }

    private void OnShowPasswordToggled(object sender, EventArgs e)
    {
        PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
        ShowPasswordButton.Text = PasswordEntry.IsPassword ? "👁️" : "🙈";
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        await AnimationService.ButtonPressAsync(LoginButton);

        // Basic validation
        if (string.IsNullOrWhiteSpace(EmailEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            ErrorLabel.Text = "Veuillez remplir tous les champs";
            ErrorBorder.IsVisible = true;
            await AnimationService.ShakeAsync(ErrorBorder);
            return;
        }

        ErrorBorder.IsVisible = false;
        
        // Show loading state
        LoginButton.Text = "Connexion...";
        LoginButton.IsEnabled = false;
        
        try
        {
            bool success = await _appState.LoginAsync(EmailEntry.Text, PasswordEntry.Text);
            
            if (success)
            {
                LoginButton.Text = "✓ Connecté";
                await AnimationService.SuccessCheckmarkAsync(LoginButton);
                await Shell.Current.GoToAsync("//HomePage");
            }
            else
            {
                await Shell.Current.GoToAsync("//AuthErrorPage");
            }
        }
        catch (Exception)
        {
            ErrorLabel.Text = "Erreur de connexion. Veuillez réessayer.";
            ErrorBorder.IsVisible = true;
            await AnimationService.ShakeAsync(ErrorBorder);
        }
        finally
        {
            LoginButton.Text = "Se connecter";
            LoginButton.IsEnabled = true;
        }
    }

    private async void OnSignupClicked(object sender, EventArgs e)
    {
        await AnimationService.ButtonPressAsync((VisualElement)sender);
        await Shell.Current.GoToAsync("//SignupPage");
    }

    private void ShowPasswordButton_Clicked(object sender, EventArgs e)
    {
        PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
        var button = (Button)sender;
        button.Text = PasswordEntry.IsPassword ? "👁" : "🙈";
    }

    private async void OnForgotPasswordClicked(object sender, EventArgs e)
    {
        await AnimationService.ButtonPressAsync((VisualElement)sender);
        await DisplayAlert("Mot de passe oublié", "Fonctionnalité de récupération à venir", "OK");
    }

    private void ForgotPasswordButton_Clicked(object sender, EventArgs e)
    {
        OnForgotPasswordClicked(sender, e);
    }

    private void LoginButton_Clicked(object sender, EventArgs e)
    {
        OnLoginClicked(sender, e);
    }

    private void SignupButton_Clicked(object sender, EventArgs e)
    {
        OnSignupClicked(sender, e);
    }
}
