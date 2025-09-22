using SmartLockerApp.Interfaces;
using SmartLockerApp.Models;

namespace SmartLockerApp.Services;

/// <summary>
/// Service de données API simplifié avec fallback vers le service local
/// </summary>
public class ApiDataService : IDataService
{
    private readonly IApiService _apiService;
    private readonly LocalDataService _fallbackService;

    public ApiDataService(IApiService apiService, LocalDataService fallbackService)
    {
        _apiService = apiService;
        _fallbackService = fallbackService;
    }

    /// <summary>
    /// Exécute une opération avec fallback automatique vers le service local
    /// </summary>
    private async Task<T> ExecuteWithFallbackAsync<T>(Func<Task<T>> operation, Func<Task<T>> fallback)
    {
        try
        {
            // TODO: Implémenter l'appel API quand disponible
            return await fallback();
        }
        catch
        {
            return await fallback();
        }
    }

    public async Task<User?> GetCurrentUserAsync() => 
        await ExecuteWithFallbackAsync(
            () => throw new NotImplementedException("API not implemented yet"),
            () => _fallbackService.GetCurrentUserAsync());

    public async Task<bool> SetCurrentUserAsync(User user) => 
        await _fallbackService.SetCurrentUserAsync(user);

    public async Task<bool> ClearCurrentUserAsync() => 
        await _fallbackService.ClearCurrentUserAsync();

    public async Task<(bool Success, User? User, string? Message)> AuthenticateAsync(string email, string password) =>
        await ExecuteWithFallbackAsync(
            () => throw new NotImplementedException("API authentication not implemented yet"),
            () => _fallbackService.AuthenticateAsync(email, password));

    public async Task<(bool Success, User? User, string? Message)> CreateAccountAsync(string firstName, string lastName, string email, string password) =>
        await ExecuteWithFallbackAsync(
            () => throw new NotImplementedException("API account creation not implemented yet"),
            () => _fallbackService.CreateAccountAsync(firstName, lastName, email, password));

    public async Task<List<Locker>> GetAvailableLockersAsync() =>
        await ExecuteWithFallbackAsync(
            () => throw new NotImplementedException("API lockers not implemented yet"),
            () => _fallbackService.GetAvailableLockersAsync());

    public async Task<Locker?> GetLockerByIdAsync(string lockerId) =>
        await ExecuteWithFallbackAsync(
            () => throw new NotImplementedException("API locker details not implemented yet"),
            () => _fallbackService.GetLockerByIdAsync(lockerId));

    public async Task<(bool Success, LockerSession? Session, string? Message)> CreateSessionAsync(string lockerId, int durationHours, List<string> items) =>
        await ExecuteWithFallbackAsync(
            () => throw new NotImplementedException("API session creation not implemented yet"),
            () => _fallbackService.CreateSessionAsync(lockerId, durationHours, items));

    public async Task<LockerSession?> GetSessionAsync(string sessionId) =>
        await ExecuteWithFallbackAsync(
            () => throw new NotImplementedException("API session details not implemented yet"),
            () => _fallbackService.GetSessionAsync(sessionId));

    public async Task<LockerSession?> GetActiveSessionAsync() =>
        await _fallbackService.GetActiveSessionAsync();

    public async Task<List<LockerSession>> GetSessionHistoryAsync() =>
        await ExecuteWithFallbackAsync(
            () => throw new NotImplementedException("API session history not implemented yet"),
            () => _fallbackService.GetSessionHistoryAsync());

    public async Task<bool> EndSessionAsync(string sessionId) =>
        await ExecuteWithFallbackAsync(
            () => throw new NotImplementedException("API session termination not implemented yet"),
            () => _fallbackService.EndSessionAsync(sessionId));

    public async Task<bool> UpdateSessionAsync(LockerSession session) =>
        await _fallbackService.UpdateSessionAsync(session);

    public async Task<decimal> CalculatePriceAsync(double hours) =>
        await ExecuteWithFallbackAsync(
            () => throw new NotImplementedException("API price calculation not implemented yet"),
            () => _fallbackService.CalculatePriceAsync(hours));
}
