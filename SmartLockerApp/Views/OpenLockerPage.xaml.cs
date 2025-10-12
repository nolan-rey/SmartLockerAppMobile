using SmartLockerApp.Services;

namespace SmartLockerApp.Views;

[QueryProperty(nameof(LockerId), "lockerId")]
[QueryProperty(nameof(DurationHours), "durationHours")]
[QueryProperty(nameof(Price), "price")]
[QueryProperty(nameof(SessionId), "sessionId")]
[QueryProperty(nameof(Action), "action")]
public partial class OpenLockerPage : ContentPage
{
    private readonly AppStateService _appState;
    private readonly HybridSessionService _hybridSessionService;
    
    public string LockerId { get; set; } = string.Empty;
    public string DurationHours { get; set; } = string.Empty;
    public string Price { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // "retrieve" pour récupération

    public OpenLockerPage(AppStateService appState, HybridSessionService hybridSessionService)
    {
        InitializeComponent();
        _appState = appState;
        _hybridSessionService = hybridSessionService;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        System.Diagnostics.Debug.WriteLine("🔓 OpenLockerPage - Simulation ouverture casier...");
        System.Diagnostics.Debug.WriteLine($"   - Casier: {LockerId}");
        System.Diagnostics.Debug.WriteLine($"   - Durée: {DurationHours}h");
        System.Diagnostics.Debug.WriteLine($"   - Prix: {Price}€");
        
        StartOpeningProcess();
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void StartOpeningProcess()
    {
        // Simulate opening process
        await Task.Delay(3000);
        
        // Update UI to show success
        StatusIcon.Text = "✅";
        StatusTitle.Text = "Casier ouvert !";
        
        if (Action == "retrieve")
        {
            StatusMessage.Text = "Récupérez vos affaires et fermez le casier";
        }
        else
        {
            StatusMessage.Text = "Vous pouvez maintenant déposer vos affaires";
        }
        
        LoadingIndicator.IsRunning = false;
        ProgressContainer.IsVisible = false;
        
        // Show instructions and actions
        InstructionsContainer.IsVisible = true;
        ActionContainer.IsVisible = true;
    }

    private async void ConfirmRetrievalButton_Clicked(object sender, EventArgs e)
    {
        if (Action == "retrieve")
        {
            // Clôturer la session via l'API
            if (!string.IsNullOrEmpty(SessionId))
            {
                DebugLogger.Section("CLÔTURE DE SESSION");
                DebugLogger.Info($"SessionId: {SessionId}");
                
                // Afficher un indicateur de chargement
                LoadingIndicator.IsRunning = true;
                ProgressContainer.IsVisible = true;
                ActionContainer.IsVisible = false;
                StatusMessage.Text = "Clôture de la session en cours...";
                
                try
                {
                    int sessionIdInt = int.Parse(SessionId);
                    var result = await _hybridSessionService.CloseSessionAsync(sessionIdInt);
                    
                    if (result.Success)
                    {
                        DebugLogger.Success("Session clôturée avec succès");
                        
                        // Navigation vers SessionClosedPage (page de confirmation simple)
                        DebugLogger.Info("Navigation vers SessionClosedPage");
                        await Shell.Current.GoToAsync("//SessionClosedPage");
                    }
                    else
                    {
                        DebugLogger.Error($"Échec clôture: {result.Message}");
                        await DisplayAlert("Erreur", $"Impossible de clôturer la session: {result.Message}", "OK");
                        await Shell.Current.GoToAsync("//HomePage");
                    }
                }
                catch (Exception ex)
                {
                    DebugLogger.Error($"Exception: {ex.Message}");
                    await DisplayAlert("Erreur", "Une erreur est survenue lors de la clôture", "OK");
                    await Shell.Current.GoToAsync("//HomePage");
                }
            }
            else
            {
                await DisplayAlert("Erreur", "Session introuvable", "OK");
                await Shell.Current.GoToAsync("//HomePage");
            }
        }
        else
        {
            // Pour le dépôt initial, naviguer vers LockInstructionsPage avec les paramètres
            DebugLogger.Info("Navigation vers LockInstructionsPage");
            await Shell.Current.GoToAsync($"//LockInstructionsPage?lockerId={LockerId}&durationHours={DurationHours}&price={Price}");
        }
    }

    private async void NeedHelpButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//HelpTutorialPage");
    }
}
