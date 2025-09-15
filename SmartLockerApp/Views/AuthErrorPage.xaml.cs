namespace SmartLockerApp.Views;

public partial class AuthErrorPage : ContentPage
{
    public AuthErrorPage()
    {
        InitializeComponent();
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void RetryButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }

    private async void ForgotPasswordButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ForgotPasswordPage");
    }

    private async void BackToLoginButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }

    public void SetErrorMessage(string message)
    {
        ErrorMessageLabel.Text = message;
    }
}
