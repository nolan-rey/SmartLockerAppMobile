namespace SmartLockerApp.Views;

public partial class UnlockInstructionsPage : ContentPage
{
    public UnlockInstructionsPage()
    {
        InitializeComponent();
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void RfidOption_Tapped(object sender, EventArgs e)
    {
        // Simulate RFID scanning
        await DisplayAlert("RFID", "Approchez votre carte RFID du lecteur...", "OK");
        await Shell.Current.GoToAsync("//UnlockConfirmationPage");
    }

    private async void FingerprintOption_Tapped(object sender, EventArgs e)
    {
        // Simulate fingerprint scanning
        await DisplayAlert("Empreinte", "Placez votre doigt sur le capteur...", "OK");
        await Shell.Current.GoToAsync("//UnlockConfirmationPage");
    }

    private async void RemoteUnlock_Tapped(object sender, EventArgs e)
    {
        // Simulate remote unlock
        await DisplayAlert("Déverrouillage à distance", "Envoi de la commande de déverrouillage...", "OK");
        await Shell.Current.GoToAsync("//UnlockConfirmationPage");
    }

    private async void NeedHelp_Tapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//HelpTutorialPage");
    }

    private async void CancelButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
