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
            CurrentUser = await _dataService.GetCurrentUserAsync();
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

        // Load all lockers (available and occupied) - we only have 2 now
        try
        {
            // Get all lockers from the data service
            var availableLockers = await _dataService.GetAvailableLockersAsync();
            
            // Also get all lockers from the locker management service to include occupied ones
            var allLockers = _lockerService.Lockers;
            
            System.Diagnostics.Debug.WriteLine($"Loading {allLockers.Count} lockers");
            
            AvailableLockers.Clear();
            foreach (var locker in allLockers)
            {
                System.Diagnostics.Debug.WriteLine($"Adding locker: {locker.Id} - {locker.Location} - {locker.Status}");
                AvailableLockers.Add(new LockerItemViewModel(locker));
            }
            
            System.Diagnostics.Debug.WriteLine($"AvailableLockers collection now has {AvailableLockers.Count} items");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading lockers: {ex.Message}");
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

    private string MapServiceIdToDisplayId(string serviceId)
    {
        return serviceId switch
        {
            "L001" => "A1",
            "L002" => "B2",
            _ => serviceId
        };
    }
}

public class LockerItemViewModel : BaseViewModel
{
    public LockerItemViewModel(Locker locker)
    {
        Locker = locker;
    }

    public Locker Locker { get; }

    public string DisplayId => CompatibilityService.IntToStringId(Locker.Id);
    public string Location => Locker.Location;
    public string Size => Locker.Size.ToString();
    public bool IsAvailable => CompatibilityService.CompareStatus(Locker.Status, LockerStatus.Available);
    public string StatusColor => IsAvailable ? "#10B981" : "#EF4444";
    public string StatusText => IsAvailable ? "Disponible" : "Occupé";

    private string MapServiceIdToDisplayId(string serviceId)
    {
        return serviceId switch
        {
            "L001" => "A1",
            "L002" => "B2",
            _ => serviceId
        };
    }
}
