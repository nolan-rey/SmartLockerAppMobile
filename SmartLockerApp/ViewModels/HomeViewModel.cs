using SmartLockerApp.Interfaces;
using SmartLockerApp.Models;
using SmartLockerApp.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SmartLockerApp.ViewModels;

public class HomeViewModel : BaseViewModel
{
    private readonly IDataService _dataService;
    private readonly LockerManagementService _lockerService;
    private User? _currentUser;
    private LockerSession? _activeSession;
    private int _totalSessions;
    private decimal _totalSpent;

    public HomeViewModel(IDataService dataService, LockerManagementService lockerService)
    {
        _dataService = dataService;
        _lockerService = lockerService;
        Title = "Accueil";
        
        AvailableLockers = new ObservableCollection<LockerItemViewModel>();
        
        // Commands
        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        LockerSelectedCommand = new RelayCommand<LockerItemViewModel>(OnLockerSelected);
        ActiveSessionCommand = new AsyncRelayCommand(NavigateToActiveSessionAsync);
        HistoryCommand = new AsyncRelayCommand(NavigateToHistoryAsync);
        SettingsCommand = new AsyncRelayCommand(NavigateToSettingsAsync);
    }

    // Properties
    public ObservableCollection<LockerItemViewModel> AvailableLockers { get; }

    public User? CurrentUser
    {
        get => _currentUser;
        set => SetProperty(ref _currentUser, value);
    }

    public LockerSession? ActiveSession
    {
        get => _activeSession;
        set => SetProperty(ref _activeSession, value);
    }

    public bool HasActiveSession => ActiveSession != null;

    public int TotalSessions
    {
        get => _totalSessions;
        set => SetProperty(ref _totalSessions, value);
    }

    public decimal TotalSpent
    {
        get => _totalSpent;
        set => SetProperty(ref _totalSpent, value);
    }

    public string WelcomeMessage => CurrentUser != null ? $"Bonjour, {CurrentUser.name.Split(' ')[0]}" : "Bonjour";

    // Commands
    public ICommand RefreshCommand { get; }
    public ICommand LockerSelectedCommand { get; }
    public ICommand ActiveSessionCommand { get; }
    public ICommand HistoryCommand { get; }
    public ICommand SettingsCommand { get; }

    // Methods
    public async Task InitializeAsync()
    {
        await ExecuteAsync(async () =>
        {
            System.Diagnostics.Debug.WriteLine("🔄 Initialisation de HomeViewModel...");
            
            CurrentUser = await _dataService.GetCurrentUserAsync();
            
            if (CurrentUser != null)
            {
                System.Diagnostics.Debug.WriteLine($"✅ Utilisateur chargé dans HomeViewModel: {CurrentUser.name} (ID: {CurrentUser.id})");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("⚠️ Aucun utilisateur chargé dans HomeViewModel");
            }
            
            await LoadDataAsync();
        });
    }

    private async Task RefreshAsync()
    {
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        // Load active session
        ActiveSession = await _dataService.GetActiveSessionAsync();
        OnPropertyChanged(nameof(HasActiveSession));

        // Load all lockers from API
        try
        {
            System.Diagnostics.Debug.WriteLine("🔄 Chargement des casiers depuis l'API...");
            
            // Get ALL lockers from the API (not just available ones)
            var allLockers = await _dataService.GetAvailableLockersAsync();
            
            if (allLockers != null && allLockers.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine($"✅ {allLockers.Count} casiers récupérés depuis l'API");
                
                AvailableLockers.Clear();
                foreach (var locker in allLockers)
                {
                    System.Diagnostics.Debug.WriteLine($"   - Casier {locker.Id}: {locker.Name} - {locker.Status}");
                    AvailableLockers.Add(new LockerItemViewModel(locker));
                }
                
                System.Diagnostics.Debug.WriteLine($"✅ {AvailableLockers.Count} casiers affichés");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("⚠️ Aucun casier récupéré depuis l'API");
                
                // Fallback: afficher un message si aucun casier n'est disponible
                if (allLockers != null && allLockers.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("ℹ️ Aucun casier disponible pour le moment");
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur chargement casiers: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"❌ Stack trace: {ex.StackTrace}");
        }

        // Load statistics
        if (CurrentUser != null)
        {
            var history = await _dataService.GetSessionHistoryAsync();
            TotalSessions = history.Count;
            TotalSpent = history.Sum(s => s.TotalCost);
        }
    }

    private void OnLockerSelected(LockerItemViewModel? lockerViewModel)
    {
        if (lockerViewModel?.Locker == null || !lockerViewModel.IsAvailable)
            return;

        // Navigate to locker detail
        var lockerId = CompatibilityService.IntToStringId(lockerViewModel.Locker.Id);
        Shell.Current.GoToAsync($"//LockerDetailPage?lockerId={lockerId}");
    }

    private async Task NavigateToActiveSessionAsync()
    {
        if (HasActiveSession)
        {
            await Shell.Current.GoToAsync("//ActiveSessionPage");
        }
    }

    private async Task NavigateToHistoryAsync()
    {
        await Shell.Current.GoToAsync("//HistoryPage");
    }

    private async Task NavigateToSettingsAsync()
    {
        await Shell.Current.GoToAsync("//SettingsPage");
    }
}

public class LockerItemViewModel : BaseViewModel
{
    public LockerItemViewModel(Locker locker)
    {
        Locker = locker;
    }

    public Locker Locker { get; }

    // Affiche "Casier 1", "Casier 2", etc. basé sur l'ID du locker dans la BDD
    public string DisplayId => $"Casier {Locker.Id}";
    
    public string Location => Locker.Location;
    public string Size => Locker.Size.ToString();
    public bool IsAvailable => Locker.IsAvailable; // Utilise la propriété IsAvailable du modèle
    public string StatusColor => IsAvailable ? "#10B981" : "#EF4444";
    public string StatusText => Locker.Status switch
    {
        "available" => "Disponible",
        "occupied" => "Occupé",
        "maintenance" => "Maintenance",
        "out_of_order" => "Hors service",
        _ => "Inconnu"
    };
}
