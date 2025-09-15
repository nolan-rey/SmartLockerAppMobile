namespace SmartLockerApp.Views;

public partial class LockInstructionsPage : ContentPage
{
    public LockInstructionsPage()
    {
        InitializeComponent();
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void RfidButton_Clicked(object sender, EventArgs e)
    {
        // Simulate RFID scanning process
        await DisplayAlert("Badge RFID", "Approchez votre badge du lecteur RFID...", "OK");
        
        // Navigate to confirmation page
        await Shell.Current.GoToAsync("//LockConfirmationPage");
    }

    private async void FingerprintButton_Clicked(object sender, EventArgs e)
    {
        // Simulate fingerprint scanning process
        await DisplayAlert("Empreinte digitale", "Placez votre doigt sur le capteur...", "OK");
        
        // Navigate to confirmation page
        await Shell.Current.GoToAsync("//LockConfirmationPage");
    }
}
