using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartLockerApp.Services;

namespace SmartLockerApp.ViewModels;

/// <summary>
/// ViewModel pour la page de session active
/// </summary>
public partial class ActiveSessionPageViewModel : BaseViewModel
{
    private readonly AppStateService _appState;

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

    #endregion

    public ActiveSessionPageViewModel(AppStateService appState)
    {
        _appState = appState;
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
        if (_appState.ActiveSession != null)
        {
            await Shell.Current.GoToAsync($"//UnlockInstructionsPage?sessionId={_appState.ActiveSession.Id}&action=close");
        }
    }

    #endregion

    #region Private Methods

    private async Task LoadSessionData()
    {
        if (_appState.ActiveSession == null)
        {
            return;
        }

        var session = _appState.ActiveSession;

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
