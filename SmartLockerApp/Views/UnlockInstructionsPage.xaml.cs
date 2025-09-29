using SmartLockerApp.Services;

namespace SmartLockerApp.Views;

public partial class UnlockInstructionsPage : ContentPage, IQueryAttributable
{
    private readonly AppStateService _appState = AppStateService.Instance;
    private string? _sessionId;
    private string? _action; // "close" pour clôturer la session

    public UnlockInstructionsPage()
    {
        InitializeComponent();
        LoadSessionInfo();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("sessionId"))
        {
            _sessionId = query["sessionId"].ToString();
        }
        if (query.ContainsKey("action"))
        {
            _action = query["action"].ToString();
        }
        LoadSessionInfo();
    }
    
    private void LoadSessionInfo()
    {
        if (_appState.ActiveSession != null)
        {
            var locker = _appState.GetLockerDetails(CompatibilityService.IntToStringId(_appState.ActiveSession.LockerId));
            if (locker != null)
            {
                // Mettre à jour les informations du casier dans l'UI si nécessaire
                Title = $"Déverrouiller {locker.Id}";
            }
        }
    }

    private async Task SimulateUnlockProcess(string method, string instruction)
    {
        // Afficher les instructions
        await DisplayAlert(method, instruction, "OK");
        
        // Simuler le processus de déverrouillage
        await DisplayAlert("Déverrouillage", "Déverrouillage en cours...", "OK");
        
        // Simuler un délai
        await Task.Delay(2000);
        
        if (_action == "close")
        {
            // Processus de clôture de session
            await DisplayAlert("Succès", "Casier déverrouillé ! Récupérez vos affaires.", "OK");
            
            // S'assurer que sessionId n'est pas null ou vide
            var sessionId = !string.IsNullOrEmpty(_sessionId) ? _sessionId : CompatibilityService.IntToStringId(_appState.ActiveSession?.Id ?? 0);
            
            if (!string.IsNullOrEmpty(sessionId))
            {
                // Naviguer vers la page de casier ouvert pour récupération
                await Shell.Current.GoToAsync($"//OpenLockerPage?sessionId={sessionId}&action=retrieve");
            }
            else
            {
                await DisplayAlert("Erreur", "Session introuvable", "OK");
                await Shell.Current.GoToAsync("//HomePage");
            }
        }
        else
        {
            // Processus normal (dépôt)
            if (_appState.ActiveSession != null)
            {
                var session = await _appState.GetSessionAsync(CompatibilityService.IntToStringId(_appState.ActiveSession.Id));
                var result = await _appState.EndSessionAsync(CompatibilityService.IntToStringId(session?.Id ?? 0));
                var success = result.Success;
                if (success)
                {
                    await DisplayAlert("Succès", "Casier déverrouillé ! Déposez vos affaires et refermez la porte.", "OK");
                    await DisplayAlert("Verrouillage", "Casier verrouillé automatiquement. Votre session est maintenant active.", "OK");
                    
                    // Naviguer vers la page de confirmation
                    await Shell.Current.GoToAsync("//UnlockConfirmationPage");
                }
                else
                {
                    await DisplayAlert("Erreur", "Impossible de verrouiller le casier", "OK");
                }
            }
        }
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void RfidOption_Tapped(object sender, EventArgs e)
    {
        await SimulateUnlockProcess("RFID", "Approchez votre badge RFID du lecteur");
    }

    private async void FingerprintOption_Tapped(object sender, EventArgs e)
    {
        await SimulateUnlockProcess("Empreinte", "Placez votre doigt sur le lecteur d'empreinte");
    }

    private async void RemoteUnlock_Tapped(object sender, EventArgs e)
    {
        await SimulateUnlockProcess("Ouverture à distance", "Ouverture du casier en cours...");
    }

    private async void NeedHelp_Tapped(object sender, EventArgs e)
    {
        await DisplayAlert("Aide", "Contactez le support au 01 23 45 67 89 pour obtenir de l'aide.", "OK");
    }

    private async void CancelButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//HomePage");
    }
}
