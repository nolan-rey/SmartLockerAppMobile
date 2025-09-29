using SmartLockerApp.Models;
using SmartLockerApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace SmartLockerApp.ViewModels;

public class ActiveSessionViewModel : BaseViewModel
{
    private readonly LocalDataService _dataService;
    private LockerSession? _activeSession;
    private string _lockerDisplayId = string.Empty;
    private string _startTime = string.Empty;
    private string _endTime = string.Empty;
    private string _duration = string.Empty;
    private string _remainingTime = string.Empty;
    private string _remainingTimeColor = "#F59E0B";

    public ActiveSessionViewModel(LocalDataService dataService)
    {
        _dataService = dataService;
        Title = "Session Active";

        // Commands
        BackCommand = new AsyncRelayCommand(GoBackAsync);
        OpenLockerCommand = new AsyncRelayCommand(OpenLockerAsync);
        EndSessionCommand = new AsyncRelayCommand(EndSessionAsync);
        RefreshCommand = new AsyncRelayCommand(RefreshSessionAsync);

        // Auto-refresh every minute
        StartAutoRefresh();
    }

    // Properties
    public LockerSession? ActiveSession
    {
        get => _activeSession;
        set
        {
            if (SetProperty(ref _activeSession, value))
            {
                UpdateDisplayProperties();
            }
        }
    }

    public string LockerDisplayId
    {
        get => _lockerDisplayId;
        set => SetProperty(ref _lockerDisplayId, value);
    }

    public string StartTime
    {
        get => _startTime;
        set => SetProperty(ref _startTime, value);
    }

    public string EndTime
    {
        get => _endTime;
        set => SetProperty(ref _endTime, value);
    }

    public string Duration
    {
        get => _duration;
        set => SetProperty(ref _duration, value);
    }

    public string RemainingTime
    {
        get => _remainingTime;
        set => SetProperty(ref _remainingTime, value);
    }

    public string RemainingTimeColor
    {
        get => _remainingTimeColor;
        set => SetProperty(ref _remainingTimeColor, value);
    }

    public bool HasActiveSession => ActiveSession != null;

    // Commands
    public ICommand BackCommand { get; }
    public ICommand OpenLockerCommand { get; }
    public ICommand EndSessionCommand { get; }
    public ICommand RefreshCommand { get; }

    // Methods
    public async Task InitializeAsync()
    {
        await ExecuteAsync(async () =>
        {
            ActiveSession = await _dataService.GetActiveSessionAsync();
        });
    }

    private async Task RefreshSessionAsync()
    {
        if (ActiveSession != null)
        {
            var updatedSession = await _dataService.GetSessionAsync(CompatibilityService.IntToStringId(ActiveSession.Id));
            if (updatedSession != null)
            {
                ActiveSession = updatedSession;
            }
        }
    }

    private void UpdateDisplayProperties()
    {
        if (ActiveSession == null)
        {
            LockerDisplayId = string.Empty;
            StartTime = string.Empty;
            EndTime = string.Empty;
            Duration = string.Empty;
            RemainingTime = string.Empty;
            return;
        }

        // Map locker ID
        LockerDisplayId = $"Casier {CompatibilityService.IntToStringId(ActiveSession.LockerId)}";

        // Format times
        StartTime = ActiveSession.StartTime.ToString("HH:mm");
        EndTime = ActiveSession.EndTime.ToString("HH:mm");

        // Format duration
        var durationHours = ActiveSession.DurationHours;
        if (durationHours < 1)
        {
            Duration = $"{(int)(durationHours * 60)} minutes";
        }
        else
        {
            Duration = $"{durationHours:F0} heure{(durationHours > 1 ? "s" : "")}";
        }

        // Calculate remaining time
        var remaining = ActiveSession.EndTime - DateTime.Now;
        if (remaining.TotalMinutes > 0)
        {
            if (remaining.TotalHours >= 1)
            {
                RemainingTime = $"{(int)remaining.TotalHours}h {remaining.Minutes}min";
            }
            else
            {
                RemainingTime = $"{(int)remaining.TotalMinutes}min";
            }
            
            // Color based on remaining time
            RemainingTimeColor = remaining.TotalMinutes < 30 ? "#EF4444" : "#F59E0B";
        }
        else
        {
            RemainingTime = "Expiré";
            RemainingTimeColor = "#EF4444";
        }

        OnPropertyChanged(nameof(HasActiveSession));
    }

    private async Task OpenLockerAsync()
    {
        if (ActiveSession != null)
        {
            await Shell.Current.GoToAsync($"//UnlockInstructionsPage?sessionId={ActiveSession.Id}");
        }
    }

    private async Task EndSessionAsync()
    {
        if (ActiveSession != null)
        {
            await Shell.Current.GoToAsync($"//UnlockInstructionsPage?sessionId={ActiveSession.Id}&action=close");
        }
    }

    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    private void StartAutoRefresh()
    {
        Microsoft.Maui.Dispatching.Dispatcher.GetForCurrentThread()?.StartTimer(TimeSpan.FromMinutes(1), () =>
        {
            if (HasActiveSession)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await RefreshSessionAsync();
                });
            }
            return HasActiveSession; // Continue timer only if there's an active session
        });
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
