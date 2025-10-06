using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartLockerApp.Services;

namespace SmartLockerApp.ViewModels;

[QueryProperty(nameof(SessionId), "sessionId")]
public partial class LockerOpenedPageViewModel : BaseViewModel
{
    private readonly AppStateService _appState;

    [ObservableProperty]
    private string sessionId = string.Empty;

    [ObservableProperty]
    private string lockerIdText = "A1";

    [ObservableProperty]
    private string durationText = "N/A";

    public LockerOpenedPageViewModel(AppStateService appState)
    {
        _appState = appState;
        Title = "Casier Ouvert";
    }

    [RelayCommand]
    private async Task LoadData()
    {
        if (string.IsNullOrEmpty(SessionId)) return;

        var session = await _appState.GetSessionAsync(SessionId);
        if (session != null)
        {
            var displayId = CompatibilityService.IntToStringId(session.LockerId);
            LockerIdText = displayId;
            DurationText = $"{session.DurationHours}h";
        }
    }

    [RelayCommand]
    private async Task Continue()
    {
        await Shell.Current.GoToAsync($"//LockInstructionsPage?sessionId={SessionId}");
    }
}
