using SmartLockerApp.Services;

namespace SmartLockerApp.Views;

public partial class SignupPage : ContentPage
{
    private readonly AppStateService _appState = AppStateService.Instance;

    public SignupPage()
    {
        InitializeComponent();
    }

    private async void SignupButton_Clicked(object sender, EventArgs e)
    {
        await AnimationService.ButtonPressAsync(SignupButton);

        // Validation des champs
        if (string.IsNullOrWhiteSpace(FullNameEntry.Text) ||
            string.IsNullOrWhiteSpace(EmailEntry.Text) ||
            string.IsNullOrWhiteSpace(PasswordEntry.Text) ||
            string.IsNullOrWhiteSpace(ConfirmPasswordEntry.Text))
        {
            await DisplayAlert("Erreur", "Veuillez remplir tous les champs obligatoires", "OK");
            return;
        }

        if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
        {
            await DisplayAlert("Erreur", "Les mots de passe ne correspondent pas", "OK");
            return;
        }

        // Extraire prénom et nom
        var nameParts = FullNameEntry.Text.Trim().Split(' ', 2);
        var firstName = nameParts[0];
        var lastName = nameParts.Length > 1 ? nameParts[1] : "";

        // Afficher l'état de chargement
        SignupButton.Text = "Création en cours...";
        SignupButton.IsEnabled = false;

        try
        {
            var (success, message) = await _appState.CreateAccountAsync(
                EmailEntry.Text.Trim(),
                PasswordEntry.Text,
                firstName,
                lastName
            );

            if (success)
            {
                SignupButton.Text = "✓ Compte créé";
                await AnimationService.SuccessCheckmarkAsync(SignupButton);
                await DisplayAlert("Succès", "Votre compte a été créé avec succès !", "OK");
                await Shell.Current.GoToAsync("//LoginPage");
            }
            else
            {
                await DisplayAlert("Erreur", message, "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Une erreur s'est produite : {ex.Message}", "OK");
        }
        finally
        {
            SignupButton.Text = "Créer mon compte";
            SignupButton.IsEnabled = true;
        }
    }

    private async void LoginButton_Clicked(object sender, EventArgs e)
    {
        await AnimationService.ButtonPressAsync(LoginButton);
        await Shell.Current.GoToAsync("//LoginPage");
    }

    private async void TestApiButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ApiTestPage");
    }
}
