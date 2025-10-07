using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartLockerApp.Interfaces;

namespace SmartLockerApp.ViewModels;

[QueryProperty(nameof(LockerId), "lockerId")]
[QueryProperty(nameof(DurationHours), "durationHours")]
[QueryProperty(nameof(Price), "price")]
public partial class LockerOpenedPageViewModel : BaseViewModel
{
    private readonly IDataService _dataService;

    [ObservableProperty]
    private string lockerId = string.Empty;

    [ObservableProperty]
    private string durationHours = string.Empty;

    [ObservableProperty]
    private string price = string.Empty;

    [ObservableProperty]
    private string lockerIdText = "Chargement...";

    [ObservableProperty]
    private string durationText = "N/A";

    public LockerOpenedPageViewModel(IDataService dataService)
    {
        _dataService = dataService;
        Title = "Casier Ouvert";
    }

    [RelayCommand]
    private async Task LoadData()
    {
        if (string.IsNullOrEmpty(LockerId)) return;

        System.Diagnostics.Debug.WriteLine($"🔓 Page Casier Ouvert:");
        System.Diagnostics.Debug.WriteLine($"   - Casier ID: {LockerId}");
        System.Diagnostics.Debug.WriteLine($"   - Durée: {DurationHours}h");
        System.Diagnostics.Debug.WriteLine($"   - Prix: {Price}€");

        // Charger les détails du casier pour affichage
        var locker = await _dataService.GetLockerByIdAsync(LockerId);
        if (locker != null)
        {
            LockerIdText = locker.Name;
            DurationText = $"{DurationHours}h";
        }
    }

    [RelayCommand]
    private async Task Continue()
    {
        System.Diagnostics.Debug.WriteLine("➡️ Navigation vers LockInstructionsPage");
        // Passer tous les paramètres à la page suivante
        await Shell.Current.GoToAsync($"//LockInstructionsPage?lockerId={LockerId}&durationHours={DurationHours}&price={Price}");
    }
}
