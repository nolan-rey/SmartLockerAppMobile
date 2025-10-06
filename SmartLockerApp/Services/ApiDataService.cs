using SmartLockerApp.Interfaces;
using SmartLockerApp.Models;

namespace SmartLockerApp.Services;

/// <summary>
/// Service de données utilisant l'API SmartLocker
/// Implémente IDataService pour être compatible avec les ViewModels existants
/// </summary>
public class ApiDataService : IDataService
{
    private readonly ApiAuthService _authService;
    private readonly ApiUserService _userService;
    private readonly ApiLockerService _lockerService;
    private readonly ApiSessionService _sessionService;
    private readonly LocalStorageService _localStorage;

    public ApiDataService(
        ApiAuthService authService,
        ApiUserService userService,
        ApiLockerService lockerService,
        ApiSessionService sessionService,
        LocalStorageService localStorage)
    {
        _authService = authService;
        _userService = userService;
        _lockerService = lockerService;
        _sessionService = sessionService;
        _localStorage = localStorage;
    }

    #region User Management

    public async Task<User?> GetCurrentUserAsync()
    {
        return await _localStorage.LoadAsync<User>("current_user");
    }

    public async Task<bool> SetCurrentUserAsync(User user)
    {
        try
        {
            await _localStorage.SaveAsync("current_user", user);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ClearCurrentUserAsync()
    {
        try
        {
            // LocalStorageService n'a pas DeleteAsync, utiliser SaveAsync avec null
            await _localStorage.SaveAsync<User?>("current_user", null);
            return true;
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region Authentication

    public async Task<(bool Success, User? User, string? Message)> AuthenticateAsync(string email, string password)
    {
        try
        {
            // Pour l'instant, utiliser les credentials de test de l'API
            var token = await _authService.LoginAsync("SaintMichel", "ITcampus");
            
            if (!string.IsNullOrEmpty(token))
            {
                // Créer un utilisateur de test pour la session
                var user = new User
                {
                    id = 1,
                    name = "Utilisateur Test",
                    email = email,
                    role = "user",
                    password_hash = "",
                    created_at = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
                
                await SetCurrentUserAsync(user);
                return (true, user, "Connexion réussie");
            }
            
            return (false, null, "Identifiants invalides");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur login API: {ex.Message}");
            return (false, null, $"Erreur de connexion: {ex.Message}");
        }
    }

    public async Task<(bool Success, User? User, string? Message)> CreateAccountAsync(string firstName, string lastName, string email, string password)
    {
        try
        {
            // Pour l'instant, créer un compte local
            var user = new User
            {
                id = new Random().Next(1000, 9999),
                name = $"{firstName} {lastName}",
                email = email,
                role = "user",
                password_hash = password, // En production, hasher le mot de passe
                created_at = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            
            await SetCurrentUserAsync(user);
            return (true, user, "Compte créé avec succès");
        }
        catch (Exception ex)
        {
            return (false, null, $"Erreur: {ex.Message}");
        }
    }

    #endregion

    #region Lockers

    public async Task<List<Locker>> GetAvailableLockersAsync()
    {
        try
        {
            var lockers = await _lockerService.GetAvailableLockersAsync();
            return lockers ?? new List<Locker>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur récupération casiers: {ex.Message}");
            return new List<Locker>();
        }
    }

    public async Task<Locker?> GetLockerByIdAsync(string lockerId)
    {
        try
        {
            // Convertir l'ID string en int pour l'API
            if (int.TryParse(lockerId, out int id))
            {
                return await _lockerService.GetLockerByIdAsync(id);
            }
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur récupération casier {lockerId}: {ex.Message}");
            return null;
        }
    }

    #endregion

    #region Sessions

    public async Task<(bool Success, LockerSession? Session, string? Message)> CreateSessionAsync(string lockerId, int durationHours, List<string> items)
    {
        try
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return (false, null, "Utilisateur non connecté");
            }

            if (!int.TryParse(lockerId, out int lockerIdInt))
            {
                return (false, null, "ID casier invalide");
            }

            var endTime = DateTime.Now.AddHours(durationHours);
            var endTimeStr = endTime.ToString("yyyy-MM-dd HH:mm:ss");
            var (success, message, session) = await _sessionService.CreateSessionAsync(user.id, lockerIdInt, endTimeStr);
            
            if (success && session != null)
            {
                return (true, session.ToLockerSession(), message);
            }
            
            return (false, null, message ?? "Erreur lors de la création de la session");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur création session: {ex.Message}");
            return (false, null, $"Erreur: {ex.Message}");
        }
    }

    public async Task<LockerSession?> GetSessionAsync(string sessionId)
    {
        try
        {
            if (int.TryParse(sessionId, out int id))
            {
                var session = await _sessionService.GetSessionByIdAsync(id);
                return session?.ToLockerSession();
            }
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur récupération session: {ex.Message}");
            return null;
        }
    }

    public async Task<LockerSession?> GetActiveSessionAsync()
    {
        try
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return null;

            var sessions = await _sessionService.GetAllSessionsAsync();
            var userSessions = sessions?.Where(s => s.UserId == user.id).ToList();
            var activeSession = userSessions?.FirstOrDefault(s => s.Status == "active" || s.Status == "locked");
            return activeSession?.ToLockerSession();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur récupération session active: {ex.Message}");
            return null;
        }
    }

    public async Task<List<LockerSession>> GetSessionHistoryAsync()
    {
        try
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return new List<LockerSession>();

            var sessions = await _sessionService.GetAllSessionsAsync();
            var userSessions = sessions?.Where(s => s.UserId == user.id).ToList();
            return userSessions.ToLockerSessions();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur récupération historique: {ex.Message}");
            return new List<LockerSession>();
        }
    }

    public async Task<bool> EndSessionAsync(string sessionId)
    {
        try
        {
            if (!int.TryParse(sessionId, out int id))
            {
                return false;
            }

            var session = await _sessionService.GetSessionByIdAsync(id);
            if (session == null) return false;

            var endedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var (success, message) = await _sessionService.UpdateSessionAsync(id, "finished", endedAt);
            return success;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur fin session: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UpdateSessionAsync(LockerSession session)
    {
        try
        {
            var endedAt = session.EndedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            var (success, message) = await _sessionService.UpdateSessionAsync(
                session.Id, 
                session.Status, 
                endedAt, 
                session.PaymentStatus);
            return success;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur mise à jour session: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region Pricing

    public async Task<decimal> CalculatePriceAsync(double hours)
    {
        // Tarification simple : 2.50€ par heure
        await Task.CompletedTask;
        return (decimal)(hours * 2.50);
    }

    #endregion
}
