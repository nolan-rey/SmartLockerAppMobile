namespace SmartLockerApp.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private void ShowPasswordButton_Clicked(object sender, EventArgs e)
    {
        PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
        ShowPasswordButton.Text = PasswordEntry.IsPassword ? "👁" : "🙈";
    }

    private async void LoginButton_Clicked(object sender, EventArgs e)
    {
        // TODO: Implement login logic
        await DisplayAlert("Login", "Fonctionnalité de connexion à implémenter", "OK");
    }

    private async void ForgotPasswordButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ForgotPasswordPage");
    }

    private async void SignupButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//SignupPage");
    }
}
