using SmartLockerApp.Services;

namespace SmartLockerApp.Views;

[QueryProperty(nameof(SessionId), "sessionId")]
public partial class LockInstructionsPage : ContentPage
{
    private readonly AppStateService _appState;
    public string SessionId { get; set; } = string.Empty;

    public LockInstructionsPage(AppStateService appState)
    {
        InitializeComponent();
        _appState = appState;
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void RfidButton_Clicked(object sender, EventArgs e)
    {
        // Simulate RFID scanning process
        await DisplayAlert("Badge RFID", "Approchez votre badge du lecteur RFID...", "OK");
        
        // Navigate to confirmation page with session ID
        await Shell.Current.GoToAsync($"//LockConfirmationPage?sessionId={SessionId}");
    }

    private async void FingerprintButton_Clicked(object sender, EventArgs e)
    {
        // Simulate fingerprint scanning process
        await DisplayAlert("Empreinte digitale", "Placez votre doigt sur le capteur...", "OK");
        
        // Navigate to confirmation page with session ID
        await Shell.Current.GoToAsync($"//LockConfirmationPage?sessionId={SessionId}");
    }
}
