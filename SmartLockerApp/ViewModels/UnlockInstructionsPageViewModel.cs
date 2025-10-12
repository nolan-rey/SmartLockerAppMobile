using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartLockerApp.Services;
using SmartLockerApp.Interfaces;
using SmartLockerApp.Models;

namespace SmartLockerApp.ViewModels;

[QueryProperty(nameof(SessionId), "sessionId")]
[QueryProperty(nameof(Action), "action")]
public partial class UnlockInstructionsPageViewModel : BaseViewModel
{
    private readonly AppStateService _appState;
    private readonly IDataService _dataService;
    private readonly ApiSessionAuthService _sessionAuthService;
    private readonly ApiAuthMethodService _authMethodService;

    [ObservableProperty]
    private string sessionId = string.Empty;

    [ObservableProperty]
    private string action = string.Empty;

    [ObservableProperty]
    private string authMethodType = string.Empty; // "rfid" ou "fingerprint"

    [ObservableProperty]
    private bool showRfidButton = false;

    [ObservableProperty]
    private bool showFingerprintButton = false;

    [ObservableProperty]
    private bool showRemoteButton = true; // Toujours disponible

    public UnlockInstructionsPageViewModel(
        AppStateService appState, 
        IDataService dataService,
        ApiSessionAuthService sessionAuthService,
        ApiAuthMethodService authMethodService)
    {
        _appState = appState;
        _dataService = dataService;
        _sessionAuthService = sessionAuthService;
        _authMethodService = authMethodService;
        Title = "Déverrouiller";
    }

    [RelayCommand]
    private async Task LoadData()
    {
        DebugLogger.Section("UNLOCK INSTRUCTIONS - LOAD DATA");
        DebugLogger.Info($"SessionId: {SessionId}");
        DebugLogger.Info($"Action: {Action}");

        // Récupérer la méthode d'authentification utilisée lors de la création de la session
        if (Action == "close" && !string.IsNullOrEmpty(SessionId))
        {
            DebugLogger.Info("Mode: Clôture de session - Chargement de la méthode d'auth");
            await LoadAuthMethodForSession();
        }
        else
        {
            DebugLogger.Info("Mode: Ouverture - Affichage de toutes les méthodes");
            // Si ce n'est pas une clôture, afficher toutes les méthodes
            ShowRfidButton = true;
            ShowFingerprintButton = true;
            ShowRemoteButton = true;
        }
    }

    private async Task LoadAuthMethodForSession()
    {
        try
        {
            int sessionIdInt = int.Parse(SessionId);
            DebugLogger.Info($"Récupération méthode d'auth pour session {sessionIdInt}");

            // Récupérer les liaisons session_auth
            var sessionAuths = await _sessionAuthService.GetSessionAuthsBySessionIdAsync(sessionIdInt);
            
            if (sessionAuths != null && sessionAuths.Any())
            {
                var sessionAuth = sessionAuths.First();
                DebugLogger.Success($"Liaison trouvée: AuthMethodId = {sessionAuth.AuthMethodId}");

                // Récupérer la méthode d'authentification
                var authMethod = await _authMethodService.GetAuthMethodByIdAsync(sessionAuth.AuthMethodId);
                
                if (authMethod != null)
                {
                    AuthMethodType = authMethod.Type.ToLower();
                    DebugLogger.Success($"Méthode d'auth: {AuthMethodType}");

                    // Afficher uniquement le bouton correspondant
                    ShowRfidButton = AuthMethodType == "rfid";
                    ShowFingerprintButton = AuthMethodType == "fingerprint";
                    ShowRemoteButton = true; // Toujours disponible
                    
                    DebugLogger.Info($"Boutons affichés: RFID={ShowRfidButton}, Fingerprint={ShowFingerprintButton}, Remote={ShowRemoteButton}");
                }
                else
                {
                    DebugLogger.Warning("Méthode d'auth non trouvée - Affichage de toutes les méthodes");
                    ShowRfidButton = true;
                    ShowFingerprintButton = true;
                    ShowRemoteButton = true;
                }
            }
            else
            {
                DebugLogger.Warning("Aucune liaison session_auth trouvée - Affichage de toutes les méthodes");
                ShowRfidButton = true;
                ShowFingerprintButton = true;
                ShowRemoteButton = true;
            }
        }
        catch (Exception ex)
        {
            DebugLogger.Error($"Erreur LoadAuthMethodForSession: {ex.Message}");
            // En cas d'erreur, afficher toutes les méthodes
            ShowRfidButton = true;
            ShowFingerprintButton = true;
            ShowRemoteButton = true;
        }
    }

    [RelayCommand]
    private async Task SelectRfid()
    {
        await UnlockWithRfid();
    }

    [RelayCommand]
    private async Task SelectFingerprint()
    {
        await UnlockWithFingerprint();
    }

    [RelayCommand]
    private async Task SelectRemoteUnlock()
    {
        await UnlockRemotely();
    }

    [RelayCommand]
    private async Task ShowHelp()
    {
        await GetHelp();
    }

    [RelayCommand]
    private async Task UnlockWithRfid()
    {
        await SimulateUnlockProcess("RFID", "Approchez votre badge RFID du lecteur");
    }

    [RelayCommand]
    private async Task UnlockWithFingerprint()
    {
        await SimulateUnlockProcess("Empreinte", "Placez votre doigt sur le lecteur d'empreinte");
    }

    [RelayCommand]
    private async Task UnlockRemotely()
    {
        await SimulateUnlockProcess("Ouverture à distance", "Ouverture du casier en cours...");
    }

    [RelayCommand]
    private async Task GetHelp()
    {
        await Shell.Current.DisplayAlert("Aide", "Contactez le support au 01 23 45 67 89 pour obtenir de l'aide.", "OK");
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await Shell.Current.GoToAsync("//HomePage");
    }

    [RelayCommand]
    private async Task NavigateBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    private async Task SimulateUnlockProcess(string method, string instruction)
    {
        await Shell.Current.DisplayAlert(method, instruction, "OK");
        await Shell.Current.DisplayAlert("Déverrouillage", "Déverrouillage en cours...", "OK");
        await Task.Delay(2000);

        if (Action == "close")
        {
            await Shell.Current.DisplayAlert("Succès", "Casier déverrouillé ! Récupérez vos affaires.", "OK");
            var sessionIdValue = !string.IsNullOrEmpty(SessionId) ? SessionId : CompatibilityService.IntToStringId(_appState.ActiveSession?.Id ?? 0);

            if (!string.IsNullOrEmpty(sessionIdValue))
            {
                await Shell.Current.GoToAsync($"//OpenLockerPage?sessionId={sessionIdValue}&action=retrieve");
            }
            else
            {
                await Shell.Current.DisplayAlert("Erreur", "Session introuvable", "OK");
                await Shell.Current.GoToAsync("//HomePage");
            }
        }
        else
        {
            if (_appState.ActiveSession != null)
            {
                var session = await _appState.GetSessionAsync(CompatibilityService.IntToStringId(_appState.ActiveSession.Id));
                var result = await _appState.EndSessionAsync(CompatibilityService.IntToStringId(session?.Id ?? 0));

                if (result.Success)
                {
                    await Shell.Current.DisplayAlert("Succès", "Casier déverrouillé ! Déposez vos affaires et refermez la porte.", "OK");
                    await Shell.Current.DisplayAlert("Verrouillage", "Casier verrouillé automatiquement. Votre session est maintenant active.", "OK");
                    await Shell.Current.GoToAsync("//UnlockConfirmationPage");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erreur", "Impossible de verrouiller le casier", "OK");
                }
            }
        }
    }
}
