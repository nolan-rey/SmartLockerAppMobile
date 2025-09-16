using SmartLockerApp.Interfaces;
using SmartLockerApp.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SmartLockerApp.ViewModels;

public class HomeViewModel : BaseViewModel
{
    private readonly IDataService _dataService;
    private User? _currentUser;
    private LockerSession? _activeSession;
    private int _totalSessions;
    private decimal _totalSpent;

    public HomeViewModel(IDataService dataService)
    {
        _dataService = dataService;
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

    public string WelcomeMessage => CurrentUser != null ? $"Bonjour, {CurrentUser.FirstName}" : "Bonjour";

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

        // Load available lockers
        var lockers = await _dataService.GetAvailableLockersAsync();
        AvailableLockers.Clear();
        foreach (var locker in lockers)
        {
            AvailableLockers.Add(new LockerItemViewModel(locker));
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
        var lockerId = MapServiceIdToDisplayId(lockerViewModel.Locker.Id);
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
            "L003" => "C3",
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

    public string DisplayId => MapServiceIdToDisplayId(Locker.Id);
    public string Location => Locker.Location;
    public string Size => Locker.Size.ToString();
    public bool IsAvailable => Locker.Status == LockerStatus.Available;
    public string StatusColor => IsAvailable ? "#10B981" : "#EF4444";
    public string StatusText => IsAvailable ? "Disponible" : "Occupé";

    private string MapServiceIdToDisplayId(string serviceId)
    {
        return serviceId switch
        {
            "L001" => "A1",
            "L002" => "B2",
            "L003" => "C3",
            _ => serviceId
        };
    }
}
