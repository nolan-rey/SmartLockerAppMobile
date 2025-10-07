using SmartLockerApp.Interfaces;
using SmartLockerApp.Models;
using SmartLockerApp.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SmartLockerApp.ViewModels;

public class DepositSetupViewModel : BaseViewModel
{
    private readonly IDataService _dataService;
    private string _lockerId = string.Empty;
    private Locker? _selectedLocker;
    private DurationOption? _selectedDuration;

    public DepositSetupViewModel(IDataService dataService)
    {
        _dataService = dataService;
        Title = "Configuration du dépôt";

        DurationOptions = new ObservableCollection<DurationOption>
        {
            new(0.5, "30 minutes", "Idéal pour courses rapides"),
            new(1.0, "1 heure", "Idéal pour shopping"),
            new(2.0, "2 heures", "Idéal pour shopping"),
            new(4.0, "4 heures", "Idéal pour journée complète")
        };

        // Set default selection
        SelectedDuration = DurationOptions.FirstOrDefault(d => d.Hours == 1.0);

        // Commands
        BackCommand = new AsyncRelayCommand(GoBackAsync);
        ConfirmCommand = new AsyncRelayCommand(ConfirmSelectionAsync, CanConfirm);
        SelectDurationCommand = new RelayCommand<DurationOption>(SelectDuration);
    }

    // Properties
    public ObservableCollection<DurationOption> DurationOptions { get; }

    public string LockerId
    {
        get => _lockerId;
        set
        {
            if (SetProperty(ref _lockerId, value))
            {
                _ = LoadLockerDetailsAsync();
            }
        }
    }

    public Locker? SelectedLocker
    {
        get => _selectedLocker;
        set => SetProperty(ref _selectedLocker, value);
    }

    public DurationOption? SelectedDuration
    {
        get => _selectedDuration;
        set
        {
            if (SetProperty(ref _selectedDuration, value))
            {
                UpdatePricing();
                ((AsyncRelayCommand)ConfirmCommand).RaiseCanExecuteChanged();
            }
        }
    }

    public string DisplayLockerId => SelectedLocker?.Name ?? "Chargement...";
    public string LocationText => SelectedLocker?.Location ?? "Chargement...";

    // Commands
    public ICommand BackCommand { get; }
    public ICommand ConfirmCommand { get; }
    public ICommand SelectDurationCommand { get; }

    // Methods
    public async Task InitializeAsync(string lockerId)
    {
        LockerId = lockerId;
        await LoadLockerDetailsAsync();
        UpdatePricing();
    }

    private async Task LoadLockerDetailsAsync()
    {
        if (string.IsNullOrEmpty(LockerId))
            return;

        System.Diagnostics.Debug.WriteLine($"🔍 Chargement des détails du casier: {LockerId}");
        
        // LockerId est maintenant l'ID de l'API (int converti en string)
        SelectedLocker = await _dataService.GetLockerByIdAsync(LockerId);
        
        if (SelectedLocker != null)
        {
            System.Diagnostics.Debug.WriteLine($"✅ Casier chargé: {SelectedLocker.Name} ({SelectedLocker.Status})");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"❌ Casier non trouvé: {LockerId}");
        }
        
        OnPropertyChanged(nameof(DisplayLockerId));
        OnPropertyChanged(nameof(LocationText));
    }

    private void UpdatePricing()
    {
        foreach (var option in DurationOptions)
        {
            option.UpdatePrice(_dataService.CalculatePriceAsync(option.Hours).Result);
        }
    }

    private void SelectDuration(DurationOption? option)
    {
        if (option != null)
        {
            SelectedDuration = option;
        }
    }

    private bool CanConfirm()
    {
        return SelectedDuration != null && SelectedLocker != null && !IsBusy;
    }

    private async Task ConfirmSelectionAsync()
    {
        if (SelectedDuration == null || SelectedLocker == null)
            return;

        // Vérifier que l'utilisateur est bien connecté
        var currentUser = await _dataService.GetCurrentUserAsync();
        if (currentUser == null)
        {
            System.Diagnostics.Debug.WriteLine("❌ Utilisateur non connecté lors de la confirmation");
            
            if (Application.Current?.Windows?.Count > 0)
            {
                var mainPage = Application.Current.Windows[0].Page;
                if (mainPage != null)
                {
                    await mainPage.DisplayAlert(
                        "Erreur", 
                        "Vous devez être connecté pour réserver un casier. Veuillez vous reconnecter.", 
                        "OK");
                    
                    // Rediriger vers la page de connexion
                    await Shell.Current.GoToAsync("//LoginPage");
                }
            }
            return;
        }

        System.Diagnostics.Debug.WriteLine($"✅ Utilisateur connecté: {currentUser.name} (ID: {currentUser.id})");
        System.Diagnostics.Debug.WriteLine($"✅ Confirmation de la session:");
        System.Diagnostics.Debug.WriteLine($"   - Casier: {SelectedLocker.Name} (ID: {SelectedLocker.Id})");
        System.Diagnostics.Debug.WriteLine($"   - Durée: {SelectedDuration.Hours} heure(s)");
        System.Diagnostics.Debug.WriteLine($"   - Prix: {SelectedDuration.Price:C}");

        // Utiliser l'ID du casier directement (int → string)
        var lockerIdString = SelectedLocker.Id.ToString();
        
        var result = await _dataService.CreateSessionAsync(
            lockerIdString, 
            (int)SelectedDuration.Hours, 
            new List<string>());

        if (result.Success && result.Session != null)
        {
            System.Diagnostics.Debug.WriteLine($"✅ Session créée avec succès: ID {result.Session.Id}");
            await Shell.Current.GoToAsync($"//LockerOpenedPage?sessionId={result.Session.Id}");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"❌ Échec création session: {result.Message}");
            
            if (Application.Current?.Windows?.Count > 0)
            {
                var mainPage = Application.Current.Windows[0].Page;
                if (mainPage != null)
                    await mainPage.DisplayAlert(
                        "Erreur", 
                        result.Message ?? "Impossible de créer la session", 
                        "OK");
            }
        }
    }

    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}

public class DurationOption : BaseViewModel
{
    private decimal _price;
    private bool _isSelected;

    public DurationOption(double hours, string displayText, string description)
    {
        Hours = hours;
        DisplayText = displayText;
        Description = description;
    }

    public double Hours { get; }
    public string DisplayText { get; }
    public string Description { get; }

    public decimal Price
    {
        get => _price;
        private set => SetProperty(ref _price, value);
    }

    public string PriceText => Price.ToString("C");

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public void UpdatePrice(decimal newPrice)
    {
        Price = newPrice;
    }
}
