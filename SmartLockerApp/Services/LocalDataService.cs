using SmartLockerApp.Interfaces;
using SmartLockerApp.Models;

namespace SmartLockerApp.Services;

/// <summary>
/// Service de données local - refactorisé depuis AppStateService pour suivre l'architecture MVVM
/// </summary>
public class LocalDataService : IDataService
{
    private readonly AuthenticationService _authService;
    private readonly LockerManagementService _lockerService;
    private readonly LocalStorageService _storageService;

    public LocalDataService()
    {
        _storageService = LocalStorageService.Instance;
        _authService = AuthenticationService.Instance;
        _lockerService = LockerManagementService.Instance;
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        var currentUser = _authService.CurrentUser;
        if (currentUser == null) return null;
        
        return new User
        {
            Id = currentUser.Id,
            FirstName = currentUser.FirstName,
            LastName = currentUser.LastName,
            Email = currentUser.Email
        };
    }

    public async Task<bool> SetCurrentUserAsync(User user)
    {
        // Cannot directly set CurrentUser as it's read-only
        // This would require authentication
        return false;
    }

    public async Task<bool> ClearCurrentUserAsync()
    {
        await _authService.LogoutAsync();
        return true;
    }

    public async Task<(bool Success, User? User, string? Message)> AuthenticateAsync(string email, string password)
    {
        var result = await _authService.LoginAsync(email, password);
        if (result.Success && _authService.CurrentUser != null)
        {
            var user = new User
            {
                Id = _authService.CurrentUser.Id,
                FirstName = _authService.CurrentUser.FirstName,
                LastName = _authService.CurrentUser.LastName,
                Email = _authService.CurrentUser.Email
            };
            return (true, user, result.Message);
        }
        return (result.Success, null, result.Message);
    }

    public async Task<(bool Success, User? User, string? Message)> CreateAccountAsync(string firstName, string lastName, string email, string password)
    {
        var result = await _authService.CreateAccountAsync(firstName, lastName, email, password);
        if (result.Success && _authService.CurrentUser != null)
        {
            var user = new User
            {
                Id = _authService.CurrentUser.Id,
                FirstName = _authService.CurrentUser.FirstName,
                LastName = _authService.CurrentUser.LastName,
                Email = _authService.CurrentUser.Email
            };
            return (true, user, result.Message);
        }
        return (result.Success, null, result.Message);
    }

    public async Task<List<Locker>> GetAvailableLockersAsync()
    {
        return _lockerService.Lockers.Where(l => l.Status == LockerStatus.Available).ToList();
    }

    public async Task<Locker?> GetLockerByIdAsync(string lockerId)
    {
        return _lockerService.GetLockerDetails(lockerId);
    }

    public async Task<(bool Success, LockerSession? Session, string? Message)> CreateSessionAsync(string lockerId, int durationHours, List<string> items)
    {
        if (_authService.CurrentUser == null)
            return (false, null, "Utilisateur non connecté");

        var result = await _lockerService.StartSessionAsync(lockerId, durationHours, items);
        return (result.Success, result.Session, result.Message);
    }

    public async Task<LockerSession?> GetSessionAsync(string sessionId)
    {
        return await _lockerService.GetSessionAsync(sessionId);
    }

    public async Task<LockerSession?> GetActiveSessionAsync()
    {
        if (_authService.CurrentUser == null)
            return null;

        return _lockerService.CurrentActiveSession;
    }

    public async Task<List<LockerSession>> GetSessionHistoryAsync()
    {
        if (_authService.CurrentUser == null)
            return new List<LockerSession>();

        return _lockerService.SessionHistory;
    }

    public async Task<bool> EndSessionAsync(string sessionId)
    {
        var result = await _lockerService.EndSessionAsync(sessionId);
        return result.Success;
    }

    public async Task<bool> UpdateSessionAsync(LockerSession session)
    {
        await _lockerService.UpdateSessionItemsAsync(session.Id, session.Items);
        return true;
    }

    public async Task<decimal> CalculatePriceAsync(double hours)
    {
        return hours switch
        {
            0.5 => 2.50m,  // 30 minutes
            1.0 => 4.00m,  // 1 heure
            2.0 => 7.00m,  // 2 heures
            4.0 => 12.00m, // 4 heures
            _ => (decimal)(hours * 4.00) // 4€ par heure par défaut
        };
    }
}
