using SmartLockerApp.Models;

namespace SmartLockerApp.Interfaces;

/// <summary>
/// Interface pour l'abstraction des données - permet de basculer entre stockage local et API
/// </summary>
public interface IDataService
{
    // User Management
    Task<User?> GetCurrentUserAsync();
    Task<bool> SetCurrentUserAsync(User user);
    Task<bool> ClearCurrentUserAsync();

    // Authentication
    Task<(bool Success, User? User, string? Message)> AuthenticateAsync(string email, string password);
    Task<(bool Success, User? User, string? Message)> CreateAccountAsync(string firstName, string lastName, string email, string password);

    // Lockers
    Task<List<Locker>> GetAvailableLockersAsync();
    Task<Locker?> GetLockerByIdAsync(string lockerId);

    // Sessions
    Task<(bool Success, LockerSession? Session, string? Message)> CreateSessionAsync(string lockerId, int durationHours, List<string> items);
    Task<LockerSession?> GetSessionAsync(string sessionId);
    Task<LockerSession?> GetActiveSessionAsync();
    Task<List<LockerSession>> GetSessionHistoryAsync();
    Task<bool> EndSessionAsync(string sessionId);
    Task<bool> UpdateSessionAsync(LockerSession session);

    // Pricing
    Task<decimal> CalculatePriceAsync(double hours);
}
