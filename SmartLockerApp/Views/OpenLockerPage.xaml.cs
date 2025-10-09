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
    public string LockerId { get; set; } = string.Empty;
    public string DurationHours { get; set; } = string.Empty;
    public string Price { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // "retrieve" pour récupération

    public OpenLockerPage(AppStateService appState)
    {
        InitializeComponent();
        _appState = appState;
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
            // Terminer la session et naviguer vers le reçu de paiement
            if (!string.IsNullOrEmpty(SessionId))
            {
                var result = await _appState.EndSessionAsync(SessionId);
                if (result.Success)
                {
                    // Naviguer vers PaymentPage avec action=receipt pour afficher le reçu
                    await Shell.Current.GoToAsync($"//PaymentPage?sessionId={SessionId}&action=receipt");
                }
                else
                {
                    await DisplayAlert("Erreur", result.Message, "OK");
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
            System.Diagnostics.Debug.WriteLine("✅ Navigation vers LockInstructionsPage avec paramètres");
            await Shell.Current.GoToAsync($"//LockInstructionsPage?lockerId={LockerId}&durationHours={DurationHours}&price={Price}");
        }
    }

    private async void NeedHelpButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//HelpTutorialPage");
    }
}
