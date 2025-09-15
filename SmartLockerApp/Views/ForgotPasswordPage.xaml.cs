namespace SmartLockerApp.Views;

public partial class ForgotPasswordPage : ContentPage
{
    public ForgotPasswordPage()
    {
        InitializeComponent();
    }

    private async void SendButton_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EmailEntry.Text))
        {
            await DisplayAlert("Erreur", "Veuillez entrer votre adresse email", "OK");
            return;
        }

        // TODO: Implement forgot password logic
        await DisplayAlert("Email envoyé", "Un lien de réinitialisation a été envoyé à votre adresse email", "OK");
    }

    private async void BackToLoginButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }
}
