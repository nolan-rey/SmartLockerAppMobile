using SmartLockerApp.Interfaces;
using SmartLockerApp.Services;

namespace SmartLockerApp.Views;

[QueryProperty(nameof(LockerId), "lockerId")]
[QueryProperty(nameof(DurationHours), "durationHours")]
[QueryProperty(nameof(Price), "price")]
[QueryProperty(nameof(AuthMethod), "authMethod")]
public partial class LockConfirmationPage : ContentPage
{
    private readonly AppStateService _appState;
    private readonly IDataService _dataService;
    private readonly HybridSessionService _hybridSessionService;
    
    public string LockerId { get; set; } = string.Empty;
    public string DurationHours { get; set; } = string.Empty;
    public string Price { get; set; } = string.Empty;
    public string AuthMethod { get; set; } = string.Empty;

    public LockConfirmationPage(AppStateService appState, IDataService dataService, HybridSessionService hybridSessionService)
    {
        InitializeComponent();
        _appState = appState;
        _dataService = dataService;
        _hybridSessionService = hybridSessionService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        System.Diagnostics.Debug.WriteLine($"✅ Page Confirmation de Verrouillage:");
        System.Diagnostics.Debug.WriteLine($"   - Casier ID: {LockerId}");
        System.Diagnostics.Debug.WriteLine($"   - Durée: {DurationHours}h");
        System.Diagnostics.Debug.WriteLine($"   - Prix: {Price}€");
        System.Diagnostics.Debug.WriteLine($"   - Méthode auth: {AuthMethod}");
        
        // 🎯 C'EST ICI qu'on crée la session dans la BDD !
        await CreateSessionInDatabaseAsync();
    }

    private async Task CreateSessionInDatabaseAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("🎯 CRÉATION DE LA SESSION DANS LA BDD...");
            
            // Vérifier l'utilisateur
            var currentUser = await _dataService.GetCurrentUserAsync();
            if (currentUser == null)
            {
                await DisplayAlert("Erreur", "Utilisateur non connecté", "OK");
                await Shell.Current.GoToAsync("//LoginPage");
                return;
            }

            // Parser la durée et l'ID du locker
            if (!int.TryParse(DurationHours, out int duration))
            {
                await DisplayAlert("Erreur", "Durée invalide", "OK");
                return;
            }
            
            if (!int.TryParse(LockerId, out int lockerIdInt))
            {
                await DisplayAlert("Erreur", "ID casier invalide", "OK");
                return;
            }

            System.Diagnostics.Debug.WriteLine($"✅ Utilisateur: {currentUser.name} (ID: {currentUser.id})");
            System.Diagnostics.Debug.WriteLine($"✅ Casier: {lockerIdInt}");
            System.Diagnostics.Debug.WriteLine($"✅ Durée: {duration} heure(s)");

            // Créer la session via HybridSessionService (qui gère automatiquement le statut du locker)
            var result = await _hybridSessionService.CreateSessionAsync(
                lockerIdInt, 
                duration, 
                2.50m); // Prix par heure

            if (result.Success && result.Session != null)
            {
                System.Diagnostics.Debug.WriteLine($"🎉 SESSION CRÉÉE AVEC SUCCÈS: ID {result.Session.Id}");
                System.Diagnostics.Debug.WriteLine($"   - User ID: {result.Session.UserId}");
                System.Diagnostics.Debug.WriteLine($"   - Locker ID: {result.Session.LockerId}");
                System.Diagnostics.Debug.WriteLine($"   - Status: {result.Session.Status}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"❌ Échec création session: {result.Message}");
                await DisplayAlert("Erreur", result.Message ?? "Impossible de créer la session", "OK");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Exception lors de la création de session: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"❌ Stack trace: {ex.StackTrace}");
            await DisplayAlert("Erreur", $"Une erreur s'est produite : {ex.Message}", "OK");
        }
    }

    private async void HomeButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//HomePage");
    }
}
