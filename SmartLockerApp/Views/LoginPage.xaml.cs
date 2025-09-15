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
        // Basic validation
        if (string.IsNullOrWhiteSpace(EmailEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            ErrorBorder.IsVisible = true;
            ErrorText.Text = "Veuillez remplir tous les champs";
            return;
        }

        // Hide error message
        ErrorBorder.IsVisible = false;

        // TODO: Implement real authentication logic
        // For now, simulate successful login and navigate to HomePage
        await Shell.Current.GoToAsync("//HomePage");
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
