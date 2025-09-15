using System.ComponentModel;

namespace SmartLockerApp.Services;

public class AppStateService : INotifyPropertyChanged
{
    private static AppStateService? _instance;
    public static AppStateService Instance => _instance ??= new AppStateService();

    private User? _currentUser;
    private LockerSession? _activeSession;
    private List<LockerSession> _sessionHistory = new();
    private List<Locker> _availableLockers = new();

    public User? CurrentUser
    {
        get => _currentUser;
        set
        {
            _currentUser = value;
            OnPropertyChanged();
        }
    }

    public LockerSession? ActiveSession
    {
        get => _activeSession;
        set
        {
            _activeSession = value;
            OnPropertyChanged();
        }
    }

    public List<LockerSession> SessionHistory
    {
        get => _sessionHistory;
        set
        {
            _sessionHistory = value;
            OnPropertyChanged();
        }
    }

    public List<Locker> AvailableLockers
    {
        get => _availableLockers;
        set
        {
            _availableLockers = value;
            OnPropertyChanged();
        }
    }

    private AppStateService()
    {
        InitializeDemoData();
    }

    private void InitializeDemoData()
    {
        // Demo user
        CurrentUser = new User
        {
            Id = "1",
            Name = "John Doe",
            Email = "john.doe@example.com",
            Phone = "+33 6 12 34 56 78"
        };

        // Demo lockers
        AvailableLockers = new List<Locker>
        {
            new() { Id = "A-12", Status = LockerStatus.Available, Size = "Medium", PricePerHour = 2.50m },
            new() { Id = "B-05", Status = LockerStatus.Occupied, Size = "Large", PricePerHour = 3.50m },
            new() { Id = "C-08", Status = LockerStatus.Available, Size = "Small", PricePerHour = 1.50m },
            new() { Id = "D-15", Status = LockerStatus.Maintenance, Size = "Medium", PricePerHour = 2.50m },
            new() { Id = "E-03", Status = LockerStatus.Available, Size = "Large", PricePerHour = 3.50m }
        };

        // Demo session history
        SessionHistory = new List<LockerSession>
        {
            new() 
            { 
                Id = "1", 
                LockerId = "A-12", 
                StartTime = DateTime.Now.AddHours(-2), 
                EndTime = DateTime.Now.AddMinutes(-30),
                Status = SessionStatus.Completed,
                TotalCost = 5.00m
            },
            new() 
            { 
                Id = "2", 
                LockerId = "C-08", 
                StartTime = DateTime.Now.AddDays(-1), 
                EndTime = DateTime.Now.AddDays(-1).AddHours(1.5),
                Status = SessionStatus.Completed,
                TotalCost = 2.25m
            }
        };
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        // Simulate API call
        await Task.Delay(1500);
        
        if (email.Contains("@") && password.Length >= 6)
        {
            CurrentUser = new User
            {
                Id = "1",
                Name = "John Doe",
                Email = email,
                Phone = "+33 6 12 34 56 78"
            };
            return true;
        }
        return false;
    }

    public async Task<LockerSession> StartSessionAsync(string lockerId, int durationHours)
    {
        await Task.Delay(1000);
        
        var locker = AvailableLockers.FirstOrDefault(l => l.Id == lockerId);
        if (locker == null) throw new Exception("Casier non trouvé");

        var session = new LockerSession
        {
            Id = Guid.NewGuid().ToString(),
            LockerId = lockerId,
            StartTime = DateTime.Now,
            PlannedEndTime = DateTime.Now.AddHours(durationHours),
            Status = SessionStatus.Active,
            EstimatedCost = locker.PricePerHour * durationHours
        };

        ActiveSession = session;
        locker.Status = LockerStatus.Occupied;
        
        return session;
    }

    public async Task<bool> EndSessionAsync()
    {
        if (ActiveSession == null) return false;

        await Task.Delay(1000);

        ActiveSession.EndTime = DateTime.Now;
        ActiveSession.Status = SessionStatus.Completed;
        
        var duration = (ActiveSession.EndTime.Value - ActiveSession.StartTime).TotalHours;
        var locker = AvailableLockers.FirstOrDefault(l => l.Id == ActiveSession.LockerId);
        ActiveSession.TotalCost = (decimal)(duration * (double)(locker?.PricePerHour ?? 2.50m));

        SessionHistory.Insert(0, ActiveSession);
        
        if (locker != null)
            locker.Status = LockerStatus.Available;

        ActiveSession = null;
        return true;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

// Models
public class User
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
}

public class Locker
{
    public string Id { get; set; } = "";
    public LockerStatus Status { get; set; }
    public string Size { get; set; } = "";
    public decimal PricePerHour { get; set; }
}

public class LockerSession
{
    public string Id { get; set; } = "";
    public string LockerId { get; set; } = "";
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public DateTime? PlannedEndTime { get; set; }
    public SessionStatus Status { get; set; }
    public decimal EstimatedCost { get; set; }
    public decimal TotalCost { get; set; }
}

public enum LockerStatus
{
    Available,
    Occupied,
    Maintenance
}

public enum SessionStatus
{
    Active,
    Completed,
    Expired
}
