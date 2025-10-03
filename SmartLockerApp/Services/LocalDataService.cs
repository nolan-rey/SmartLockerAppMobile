using SmartLockerApp.Interfaces;
using SmartLockerApp.Models;

namespace SmartLockerApp.Services;

/// <summary>
/// Service de données local optimisé utilisant les services existants
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

    /// <summary>
    /// Convertit un UserAccount en User
    /// </summary>
    private static User? ConvertToUser(UserAccount? userAccount) =>
        userAccount == null ? null : new User
        {
            id = CompatibilityService.StringToIntId(userAccount.Id),
            name = $"{userAccount.FirstName} {userAccount.LastName}".Trim(),
            email = userAccount.Email,
            role = "user",
            password_hash = "",
            created_at = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

    public async Task<User?> GetCurrentUserAsync() =>
        await Task.FromResult(ConvertToUser(_authService.CurrentUser));

    public async Task<bool> SetCurrentUserAsync(User user) =>
        // Cannot directly set CurrentUser as it's read-only - requires authentication
        await Task.FromResult(false);

    public async Task<bool> ClearCurrentUserAsync()
    {
        await _authService.LogoutAsync();
        return true;
    }

    public async Task<(bool Success, User? User, string? Message)> AuthenticateAsync(string email, string password)
    {
        var result = await _authService.LoginAsync(email, password);
        return (result.Success, ConvertToUser(_authService.CurrentUser), result.Message);
    }

    public async Task<(bool Success, User? User, string? Message)> CreateAccountAsync(string firstName, string lastName, string email, string password)
    {
        var result = await _authService.CreateAccountAsync(email, password, firstName, lastName);
        return (result.Success, ConvertToUser(_authService.CurrentUser), result.Message);
    }

    public async Task<List<Locker>> GetAvailableLockersAsync() =>
        await Task.FromResult(_lockerService.Lockers.Where(l => CompatibilityService.CompareStatus(l.Status, LockerStatus.Available)).ToList());

    public async Task<Locker?> GetLockerByIdAsync(string lockerId) =>
        await Task.FromResult(_lockerService.GetLockerDetails(CompatibilityService.StringToIntId(lockerId).ToString()));

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

    public async Task<LockerSession?> GetActiveSessionAsync() =>
        await Task.FromResult(_authService.CurrentUser == null ? null : _lockerService.CurrentActiveSession);

    public async Task<List<LockerSession>> GetSessionHistoryAsync() =>
        await Task.FromResult(_authService.CurrentUser == null ? new List<LockerSession>() : _lockerService.SessionHistory);

    public async Task<bool> EndSessionAsync(string sessionId)
    {
        var result = await _lockerService.EndSessionAsync(sessionId);
        return result.Success;
    }

    public async Task<bool> UpdateSessionAsync(LockerSession session)
    {
        await _lockerService.UpdateSessionItemsAsync(CompatibilityService.IntToStringId(session.Id), session.Items);
        return true;
    }

    public async Task<decimal> CalculatePriceAsync(double hours) =>
        await Task.FromResult(hours switch
        {
            0.5 => 2.50m,  // 30 minutes
            1.0 => 4.00m,  // 1 heure
            2.0 => 7.00m,  // 2 heures
            4.0 => 12.00m, // 4 heures
            _ => (decimal)(hours * 4.00) // 4€ par heure par défaut
        });
}
