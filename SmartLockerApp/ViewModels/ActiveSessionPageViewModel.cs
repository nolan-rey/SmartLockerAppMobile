using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartLockerApp.Models;
using SmartLockerApp.Services;

namespace SmartLockerApp.ViewModels;

/// <summary>
/// ViewModel pour la page de session active
/// </summary>
public partial class ActiveSessionPageViewModel : BaseViewModel
{
    private readonly AppStateService _appState;
    private readonly AuthenticationService _authService;
    private readonly HybridSessionService _hybridSessionService;

    #region Observable Properties

    [ObservableProperty]
    private string lockerIdText = "Casier";

    [ObservableProperty]
    private string startTimeText = "N/A";

    [ObservableProperty]
    private string endTimeText = "N/A";

    [ObservableProperty]
    private string durationText = "N/A";

    [ObservableProperty]
    private string remainingTimeText = "N/A";

    [ObservableProperty]
    private Color remainingTimeColor = Color.FromArgb("#F59E0B");

    // Stocker l'ID de la session pour éviter les problèmes de null
    private int? _currentSessionId;

    #endregion

    public ActiveSessionPageViewModel(
        AppStateService appState, 
        AuthenticationService authService,
        HybridSessionService hybridSessionService)
    {
        _appState = appState;
        _authService = authService;
        _hybridSessionService = hybridSessionService;
        Title = "Session Active";
    }

    #region Commands

    [RelayCommand]
    private async Task LoadData()
    {
        await LoadSessionData();
    }

    [RelayCommand]
    private async Task NavigateBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task EndSession()
    {
        DebugLogger.Section("END SESSION - CLÔTURE");
        
        // Utiliser l'ID stocké, sinon récupérer depuis l'API
        int sessionId = _currentSessionId ?? 0;
        
        if (sessionId == 0)
        {
            DebugLogger.Warning("SessionId non stocké, récupération depuis l'API...");
            var apiSession = await _hybridSessionService.GetMyActiveSessionAsync(forceRefresh: true);
            
            if (apiSession != null)
            {
                sessionId = apiSession.Id;
                DebugLogger.Success($"Session récupérée: ID={sessionId}");
            }
        }
        
        if (sessionId > 0)
        {
            DebugLogger.Success($"Navigation vers UnlockInstructionsPage (sessionId={sessionId})");
            await Shell.Current.GoToAsync($"//UnlockInstructionsPage?sessionId={sessionId}&action=close");
        }
        else
        {
            DebugLogger.Error("Aucune session trouvée");
            await Shell.Current.DisplayAlert("Info", "Vous n'avez pas de session active", "OK");
            await Shell.Current.GoToAsync("//HomePage");
        }
    }

    #endregion

    #region Private Methods

    private async Task LoadSessionData()
    {
        DebugLogger.Section("LOAD SESSION DATA - DÉBUT");
        
        // Récupérer directement la session depuis l'API (pas besoin de vérifier CurrentUser)
        // L'utilisateur est déjà authentifié via JWT pour accéder à cette page
        var apiSession = await _hybridSessionService.GetMyActiveSessionAsync(forceRefresh: true);
        
        if (apiSession == null)
        {
            DebugLogger.Warning("Aucune session active trouvée");
            await Shell.Current.DisplayAlert("Info", "Vous n'avez pas de session active", "OK");
            await Shell.Current.GoToAsync("//HomePage");
            return;
        }
        
        // Convertir en LockerSession pour compatibilité
        var session = _hybridSessionService.ConvertToLockerSession(apiSession);
        
        if (session == null)
        {
            DebugLogger.Error("ERREUR DE CONVERSION DE LA SESSION");
            return;
        }
        
        // Stocker l'ID de la session
        _currentSessionId = session.Id;
        DebugLogger.Success($"Session chargée: ID={_currentSessionId}, Locker={session.LockerId}");

        // Mapper l'ID du casier pour l'affichage
        var displayLockerId = CompatibilityService.IntToStringId(session.LockerId);
        LockerIdText = $"Casier {displayLockerId}";

        // Afficher les heures de début et fin
        StartTimeText = session.StartTime.ToString("HH:mm");
        EndTimeText = session.EndTime.ToString("HH:mm");

        // Afficher la durée
        var duration = session.DurationHours;
        if (duration < 1)
        {
            DurationText = $"{(int)(duration * 60)} minutes";
        }
        else
        {
            DurationText = $"{duration:F0} heure{(duration > 1 ? "s" : "")}";
        }

        // Calculer et afficher le temps restant
        var remaining = session.EndTime - DateTime.Now;
        if (remaining.TotalMinutes > 0)
        {
            if (remaining.TotalHours >= 1)
            {
                RemainingTimeText = $"{(int)remaining.TotalHours}h {remaining.Minutes}min";
            }
            else
            {
                RemainingTimeText = $"{(int)remaining.TotalMinutes}min";
            }
            RemainingTimeColor = Color.FromArgb("#F59E0B");
        }
        else
        {
            RemainingTimeText = "Expiré";
            RemainingTimeColor = Color.FromArgb("#EF4444");
        }

        await Task.CompletedTask;
    }

    #endregion
}
