using SmartLockerApp.Services;
using SmartLockerApp.Interfaces;

namespace SmartLockerApp.Views;

public partial class SignupPage : ContentPage
{
    private readonly IDataService _dataService;

    public SignupPage(IDataService dataService)
    {
        InitializeComponent();
        _dataService = dataService;
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
            System.Diagnostics.Debug.WriteLine($"📝 Début création de compte pour: {EmailEntry.Text.Trim()}");
            
            var (success, user, message) = await _dataService.CreateAccountAsync(
                firstName,
                lastName,
                EmailEntry.Text.Trim(),
                PasswordEntry.Text
            );

            if (success)
            {
                System.Diagnostics.Debug.WriteLine($"✅ Compte créé avec succès: {user?.name} (ID: {user?.id})");
                
                SignupButton.Text = "✓ Compte créé";
                await AnimationService.SuccessCheckmarkAsync(SignupButton);
                await DisplayAlert("Succès", message ?? "Votre compte a été créé avec succès !", "OK");
                await Shell.Current.GoToAsync("//LoginPage");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"❌ Échec création de compte: {message}");
                await DisplayAlert("Erreur", message ?? "Une erreur s'est produite", "OK");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Exception lors de la création de compte: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"❌ Stack trace: {ex.StackTrace}");
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
}
