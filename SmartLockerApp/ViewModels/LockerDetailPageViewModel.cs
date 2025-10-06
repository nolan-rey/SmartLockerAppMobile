using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartLockerApp.Models;
using SmartLockerApp.Services;

namespace SmartLockerApp.ViewModels;

/// <summary>
/// ViewModel pour la page de détail d'un casier
/// </summary>
[QueryProperty(nameof(LockerId), "lockerId")]
public partial class LockerDetailPageViewModel : BaseViewModel
{
    private readonly AppStateService _appState;

    #region Observable Properties

    [ObservableProperty]
    private string lockerId = string.Empty;

    [ObservableProperty]
    private Locker? selectedLocker;

    [ObservableProperty]
    private string statusText = "Disponible";

    [ObservableProperty]
    private Color statusColor = Color.FromArgb("#10B981");

    [ObservableProperty]
    private bool isAvailable = true;

    #endregion

    public LockerDetailPageViewModel(AppStateService appState)
    {
        _appState = appState;
        Title = "Détail du casier";
    }

    #region Commands

    [RelayCommand]
    private void LoadData()
    {
        LoadLockerData();
        UpdateStatusDisplay();
    }

    [RelayCommand]
    private async Task NavigateBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task UseLocker()
    {
        if (SelectedLocker == null || !IsAvailable) return;

        await Shell.Current.GoToAsync($"//DepositSetupPage?lockerId={SelectedLocker.Id}");
    }

    [RelayCommand]
    private async Task OpenRemotely()
    {
        if (SelectedLocker == null) return;

        IsBusy = true;

        try
        {
            await Task.Delay(2000); // Simuler appel API
            await Shell.Current.DisplayAlert("Succès", $"Le casier {SelectedLocker.Id} a été ouvert à distance", "OK");
            await Shell.Current.GoToAsync("//OpenLockerPage");
        }
        catch
        {
            await Shell.Current.DisplayAlert("Erreur", "Impossible d'ouvrir le casier à distance", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    #endregion

    #region Private Methods

    private void LoadLockerData()
    {
        if (!string.IsNullOrEmpty(LockerId))
        {
            SelectedLocker = CreateDemoLocker(LockerId);
        }
        else
        {
            SelectedLocker = _appState.Lockers.FirstOrDefault() ?? CreateDemoLocker("A1");
        }

        if (SelectedLocker != null)
        {
            Title = $"Casier {SelectedLocker.Id}";
        }
    }

    private Locker CreateDemoLocker(string lockerId)
    {
        var locations = new Dictionary<string, string>
        {
            { "A1", "Entrée principale" },
            { "B2", "Hall principal" }
        };

        var status = LockerStatus.Available;

        return new Locker
        {
            Id = CompatibilityService.StringToIntId(lockerId),
            Name = locations.GetValueOrDefault(lockerId, "Emplacement inconnu"),
            Size = LockerSize.Medium,
            Status = CompatibilityService.StatusToString(status),
            PricePerHour = 2.50m,
            Features = new List<string> { "Sécurisé", "Climatisé", "Accès 24/7" }
        };
    }

    private void UpdateStatusDisplay()
    {
        if (SelectedLocker == null) return;

        var status = CompatibilityService.StringToStatus(SelectedLocker.Status);
        IsAvailable = status == LockerStatus.Available;

        StatusText = status switch
        {
            LockerStatus.Available => "Disponible",
            LockerStatus.Occupied => "Occupé",
            LockerStatus.Maintenance => "Maintenance",
            _ => "Inconnu"
        };

        StatusColor = status switch
        {
            LockerStatus.Available => Color.FromArgb("#10B981"),
            LockerStatus.Occupied => Color.FromArgb("#EF4444"),
            LockerStatus.Maintenance => Color.FromArgb("#F59E0B"),
            _ => Color.FromArgb("#64748B")
        };
    }

    #endregion
}
