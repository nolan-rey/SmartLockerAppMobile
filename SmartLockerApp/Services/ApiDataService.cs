using SmartLockerApp.Interfaces;
using SmartLockerApp.Models;

namespace SmartLockerApp.Services;

/// <summary>
/// Implémentation future pour l'API - actuellement utilise le service local
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

    public async Task<User?> GetCurrentUserAsync()
    {
        // Pour l'instant, utilise le service local
        // TODO: Implémenter avec l'API quand disponible
        return await _fallbackService.GetCurrentUserAsync();
    }

    public async Task<bool> SetCurrentUserAsync(User user)
    {
        return await _fallbackService.SetCurrentUserAsync(user);
    }

    public async Task<bool> ClearCurrentUserAsync()
    {
        return await _fallbackService.ClearCurrentUserAsync();
    }

    public async Task<(bool Success, User? User, string? Message)> AuthenticateAsync(string email, string password)
    {
        try
        {
            // TODO: Remplacer par l'appel API
            // var response = await _apiService.LoginAsync(email, password);
            // if (response.Success && response.Data != null)
            // {
            //     await SetCurrentUserAsync(response.Data);
            //     return (true, response.Data, null);
            // }
            // return (false, null, response.Message);

            // Utilise le service local pour l'instant
            return await _fallbackService.AuthenticateAsync(email, password);
        }
        catch (Exception ex)
        {
            return (false, null, $"Erreur de connexion: {ex.Message}");
        }
    }

    public async Task<(bool Success, User? User, string? Message)> CreateAccountAsync(string firstName, string lastName, string email, string password)
    {
        try
        {
            // TODO: Remplacer par l'appel API
            return await _fallbackService.CreateAccountAsync(firstName, lastName, email, password);
        }
        catch (Exception ex)
        {
            return (false, null, $"Erreur de création de compte: {ex.Message}");
        }
    }

    public async Task<List<Locker>> GetAvailableLockersAsync()
    {
        try
        {
            // TODO: Remplacer par l'appel API
            return await _fallbackService.GetAvailableLockersAsync();
        }
        catch (Exception ex)
        {
            // Fallback vers le service local en cas d'erreur
            return await _fallbackService.GetAvailableLockersAsync();
        }
    }

    public async Task<Locker?> GetLockerByIdAsync(string lockerId)
    {
        try
        {
            // TODO: Remplacer par l'appel API
            return await _fallbackService.GetLockerByIdAsync(lockerId);
        }
        catch (Exception ex)
        {
            return await _fallbackService.GetLockerByIdAsync(lockerId);
        }
    }

    public async Task<(bool Success, LockerSession? Session, string? Message)> CreateSessionAsync(string lockerId, int durationHours, List<string> items)
    {
        try
        {
            // TODO: Remplacer par l'appel API
            return await _fallbackService.CreateSessionAsync(lockerId, durationHours, items);
        }
        catch (Exception ex)
        {
            return (false, null, $"Erreur de création de session: {ex.Message}");
        }
    }

    public async Task<LockerSession?> GetSessionAsync(string sessionId)
    {
        try
        {
            // TODO: Remplacer par l'appel API
            return await _fallbackService.GetSessionAsync(sessionId);
        }
        catch (Exception ex)
        {
            return await _fallbackService.GetSessionAsync(sessionId);
        }
    }

    public async Task<LockerSession?> GetActiveSessionAsync()
    {
        return await _fallbackService.GetActiveSessionAsync();
    }

    public async Task<List<LockerSession>> GetSessionHistoryAsync()
    {
        try
        {
            // TODO: Remplacer par l'appel API
            return await _fallbackService.GetSessionHistoryAsync();
        }
        catch (Exception ex)
        {
            return await _fallbackService.GetSessionHistoryAsync();
        }
    }

    public async Task<bool> EndSessionAsync(string sessionId)
    {
        try
        {
            // TODO: Remplacer par l'appel API
            return await _fallbackService.EndSessionAsync(sessionId);
        }
        catch (Exception ex)
        {
            return await _fallbackService.EndSessionAsync(sessionId);
        }
    }

    public async Task<bool> UpdateSessionAsync(LockerSession session)
    {
        return await _fallbackService.UpdateSessionAsync(session);
    }

    public async Task<decimal> CalculatePriceAsync(double hours)
    {
        try
        {
            // TODO: Remplacer par l'appel API
            return await _fallbackService.CalculatePriceAsync(hours);
        }
        catch (Exception ex)
        {
            return await _fallbackService.CalculatePriceAsync(hours);
        }
    }
}
