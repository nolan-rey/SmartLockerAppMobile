using SmartLockerApp.Services;
using SmartLockerApp.Interfaces;

namespace SmartLockerApp.Views;

[QueryProperty(nameof(LockerId), "lockerId")]
[QueryProperty(nameof(DurationHours), "durationHours")]
[QueryProperty(nameof(Price), "price")]
public partial class LockInstructionsPage : ContentPage
{
    private readonly AppStateService _appState;
    private readonly IDataService _dataService;
    
    public string LockerId { get; set; } = string.Empty;
    public string DurationHours { get; set; } = string.Empty;
    public string Price { get; set; } = string.Empty;

    public LockInstructionsPage(AppStateService appState, IDataService dataService)
    {
        InitializeComponent();
        _appState = appState;
        _dataService = dataService;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        System.Diagnostics.Debug.WriteLine($"🔐 Page Instructions de Verrouillage:");
        System.Diagnostics.Debug.WriteLine($"   - Casier ID: {LockerId}");
        System.Diagnostics.Debug.WriteLine($"   - Durée: {DurationHours}h");
        System.Diagnostics.Debug.WriteLine($"   - Prix: {Price}€");
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void RfidButton_Clicked(object sender, EventArgs e)
    {
        // Simulate RFID scanning process
        await DisplayAlert("Badge RFID", "Approchez votre badge du lecteur RFID...", "OK");
        
        System.Diagnostics.Debug.WriteLine("🎫 Méthode d'authentification: RFID");
        
        // ✅ CORRECTION: Navigate to confirmation page WITHOUT sessionId (session not created yet)
        await Shell.Current.GoToAsync($"//LockConfirmationPage?lockerId={LockerId}&durationHours={DurationHours}&price={Price}&authMethod=rfid");
    }

    private async void FingerprintButton_Clicked(object sender, EventArgs e)
    {
        // Simulate fingerprint scanning process
        await DisplayAlert("Empreinte digitale", "Placez votre doigt sur le capteur...", "OK");
        
        System.Diagnostics.Debug.WriteLine("👆 Méthode d'authentification: Fingerprint");
        
        // ✅ CORRECTION: Navigate to confirmation page WITHOUT sessionId (session not created yet)
        await Shell.Current.GoToAsync($"//LockConfirmationPage?lockerId={LockerId}&durationHours={DurationHours}&price={Price}&authMethod=fingerprint");
    }
}
