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
    
    public string LockerId { get; set; } = string.Empty;
    public string DurationHours { get; set; } = string.Empty;
    public string Price { get; set; } = string.Empty;
    public string AuthMethod { get; set; } = string.Empty;

    public LockConfirmationPage(AppStateService appState, IDataService dataService)
    {
        InitializeComponent();
        _appState = appState;
        _dataService = dataService;
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

            // Parser la durée
            if (!int.TryParse(DurationHours, out int duration))
            {
                await DisplayAlert("Erreur", "Durée invalide", "OK");
                return;
            }

            System.Diagnostics.Debug.WriteLine($"✅ Utilisateur: {currentUser.name} (ID: {currentUser.id})");
            System.Diagnostics.Debug.WriteLine($"✅ Casier: {LockerId}");
            System.Diagnostics.Debug.WriteLine($"✅ Durée: {duration} heure(s)");

            // Créer la session via l'API
            var result = await _dataService.CreateSessionAsync(
                LockerId, 
                duration, 
                new List<string>()); // Items vides pour l'instant

            if (result.Success && result.Session != null)
            {
                System.Diagnostics.Debug.WriteLine($"🎉 SESSION CRÉÉE AVEC SUCCÈS: ID {result.Session.Id}");
                System.Diagnostics.Debug.WriteLine($"   - User ID: {result.Session.UserId}");
                System.Diagnostics.Debug.WriteLine($"   - Locker ID: {result.Session.LockerId}");
                System.Diagnostics.Debug.WriteLine($"   - Status: {result.Session.Status}");
                System.Diagnostics.Debug.WriteLine($"   - Start: {result.Session.StartedAt}");
                System.Diagnostics.Debug.WriteLine($"   - Planned End: {result.Session.PlannedEndAt}");
                System.Diagnostics.Debug.WriteLine($"   - Amount: {result.Session.AmountDue}€");
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
