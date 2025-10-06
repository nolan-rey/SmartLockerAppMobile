using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartLockerApp.Services;

namespace SmartLockerApp.ViewModels;

[QueryProperty(nameof(SessionId), "sessionId")]
[QueryProperty(nameof(Action), "action")]
public partial class UnlockInstructionsPageViewModel : BaseViewModel
{
    private readonly AppStateService _appState;

    [ObservableProperty]
    private string sessionId = string.Empty;

    [ObservableProperty]
    private string action = string.Empty;

    public UnlockInstructionsPageViewModel(AppStateService appState)
    {
        _appState = appState;
        Title = "Déverrouiller";
    }

    [RelayCommand]
    private async Task LoadData()
    {
        if (_appState.ActiveSession != null)
        {
            var locker = _appState.GetLockerDetails(CompatibilityService.IntToStringId(_appState.ActiveSession.LockerId));
            if (locker != null)
            {
                Title = $"Déverrouiller {locker.Id}";
            }
        }
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
