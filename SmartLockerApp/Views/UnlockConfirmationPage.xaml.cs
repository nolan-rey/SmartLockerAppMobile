using SmartLockerApp.Services;
using SmartLockerApp.Interfaces;

namespace SmartLockerApp.Views;

[QueryProperty(nameof(SessionId), "sessionId")]
[QueryProperty(nameof(Action), "action")]
public partial class UnlockConfirmationPage : ContentPage
{
    private readonly AppStateService _appState;
    private readonly IDataService _dataService;
    
    public string SessionId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;

    public UnlockConfirmationPage(AppStateService appState, IDataService dataService)
    {
        InitializeComponent();
        _appState = appState;
        _dataService = dataService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        System.Diagnostics.Debug.WriteLine($"✅ UnlockConfirmationPage - OnAppearing");
        System.Diagnostics.Debug.WriteLine($"   - SessionId: {SessionId}");
        System.Diagnostics.Debug.WriteLine($"   - Action: {Action}");

        // Si action=close, clôturer la session via l'API
        if (Action == "close" && !string.IsNullOrEmpty(SessionId))
        {
            await CloseSessionAsync();
        }
    }

    private async Task CloseSessionAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"🔒 Clôture de la session {SessionId} via API...");
            
            var result = await _appState.EndSessionAsync(SessionId);
            
            if (result.Success)
            {
                System.Diagnostics.Debug.WriteLine($"✅ Session {SessionId} clôturée avec succès");
                await DisplayAlert("Succès", "Votre session a été clôturée avec succès.", "OK");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"❌ Échec clôture session: {result.Message}");
                await DisplayAlert("Erreur", $"Impossible de clôturer la session: {result.Message}", "OK");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur CloseSessionAsync: {ex.Message}");
            await DisplayAlert("Erreur", $"Une erreur s'est produite: {ex.Message}", "OK");
        }
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void ContinueButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//OpenLockerPage");
    }

    private async void ViewSessionButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ActiveSessionPage");
    }

    private async void BackToHomeButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//HomePage");
    }
}
