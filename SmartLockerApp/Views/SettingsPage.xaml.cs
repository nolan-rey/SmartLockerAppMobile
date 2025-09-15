namespace SmartLockerApp.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void EditProfileButton_Clicked(object sender, EventArgs e)
    {
        // TODO: Navigate to profile edit page
        await DisplayAlert("Profil", "Modification du profil à implémenter", "OK");
    }

    private async void ChangePasswordButton_Tapped(object sender, EventArgs e)
    {
        // TODO: Navigate to change password page
        await DisplayAlert("Mot de passe", "Changement de mot de passe à implémenter", "OK");
    }

    private async void HelpButton_Tapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//HelpTutorialPage");
    }

    private async void ContactButton_Tapped(object sender, EventArgs e)
    {
        // TODO: Implement contact support
        await DisplayAlert("Support", "Contacter le support à implémenter", "OK");
    }

    private async void AboutButton_Tapped(object sender, EventArgs e)
    {
        await DisplayAlert("À propos", "SmartLocker App v1.0\n\nApplication de gestion de casiers intelligents", "OK");
    }

    private async void LogoutButton_Clicked(object sender, EventArgs e)
    {
        bool result = await DisplayAlert("Déconnexion", "Êtes-vous sûr de vouloir vous déconnecter?", "Oui", "Non");
        if (result)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
