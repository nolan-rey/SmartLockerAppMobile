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
        try
        {
            var user = await _localStorage.LoadAsync<User>("current_user");
            
            if (user != null)
            {
                System.Diagnostics.Debug.WriteLine($"✅ Utilisateur chargé depuis le stockage: {user.name} (ID: {user.id})");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("⚠️ Aucun utilisateur trouvé dans le stockage");
            }
            
            return user;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur chargement utilisateur: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> SetCurrentUserAsync(User user)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"💾 Sauvegarde de l'utilisateur: {user.name} (ID: {user.id})");
            await _localStorage.SaveAsync("current_user", user);
            System.Diagnostics.Debug.WriteLine("✅ Utilisateur sauvegardé avec succès");
            
            // Vérifier immédiatement après la sauvegarde
            var savedUser = await _localStorage.LoadAsync<User>("current_user");
            if (savedUser != null)
            {
                System.Diagnostics.Debug.WriteLine($"✅ Vérification: Utilisateur bien sauvegardé: {savedUser.name}");
                return true;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("❌ Vérification: L'utilisateur n'a pas été sauvegardé !");
                return false;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur sauvegarde utilisateur: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"🔐 Tentative de connexion pour: {email}");
            
            // 1. Récupérer le token JWT avec les credentials de production
            var token = await _authService.GetValidTokenAsync();
            
            if (string.IsNullOrEmpty(token))
            {
                System.Diagnostics.Debug.WriteLine("❌ Impossible d'obtenir le token JWT");
                return (false, null, "Erreur de connexion à l'API. Vérifiez votre connexion Internet.");
            }

            System.Diagnostics.Debug.WriteLine("✅ Token JWT obtenu");

            // 2. Chercher l'utilisateur par email dans l'API
            var apiUser = await _userService.GetUserByEmailAsync(email);
            
            if (apiUser != null)
            {
                System.Diagnostics.Debug.WriteLine($"✅ Utilisateur trouvé: {apiUser.name} (ID: {apiUser.id})");
                
                // TODO: Vérifier le mot de passe hashé côté serveur
                // Pour l'instant, on accepte la connexion si l'utilisateur existe
                await SetCurrentUserAsync(apiUser);
                return (true, apiUser, "Connexion réussie");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"❌ Aucun utilisateur trouvé avec l'email: {email}");
                return (false, null, "Email ou mot de passe incorrect. Veuillez créer un compte si vous n'en avez pas.");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur login API: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"❌ Stack trace: {ex.StackTrace}");
            return (false, null, $"Erreur de connexion: {ex.Message}");
        }
    }

    public async Task<(bool Success, User? User, string? Message)> CreateAccountAsync(string firstName, string lastName, string email, string password)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"📝 Création de compte pour: {email}");
            
            // 1. Obtenir le token JWT pour accéder à l'API
            var token = await _authService.GetValidTokenAsync();
            
            if (string.IsNullOrEmpty(token))
            {
                System.Diagnostics.Debug.WriteLine("⚠️ Impossible d'obtenir le token JWT, création locale uniquement");
                // Fallback: créer un compte local
                var localUser = new User
                {
                    id = new Random().Next(1000, 9999),
                    name = $"{firstName} {lastName}",
                    email = email,
                    role = "user",
                    password_hash = password,
                    created_at = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
                
                await SetCurrentUserAsync(localUser);
                return (true, localUser, "Compte créé localement (API indisponible)");
            }

            // 2. Créer l'utilisateur dans l'API
            var fullName = $"{firstName} {lastName}";
            var (success, message, apiUser) = await _userService.CreateUserAsync(
                name: fullName,
                email: email,
                passwordHash: password, // TODO: Hasher le mot de passe
                role: "user"
            );

            if (success && apiUser != null)
            {
                System.Diagnostics.Debug.WriteLine($"✅ Utilisateur créé dans l'API avec ID={apiUser.id}");
                
                // Sauvegarder localement aussi
                await SetCurrentUserAsync(apiUser);
                
                return (true, apiUser, "Compte créé avec succès !");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ Échec création API: {message}");
                
                // Fallback: créer un compte local quand même
                var localUser = new User
                {
                    id = new Random().Next(1000, 9999),
                    name = fullName,
                    email = email,
                    role = "user",
                    password_hash = password,
                    created_at = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
                
                await SetCurrentUserAsync(localUser);
                return (true, localUser, $"Compte créé localement ({message})");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur création compte: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"📝 Création de session:");
            System.Diagnostics.Debug.WriteLine($"   - Casier ID: {lockerId}");
            System.Diagnostics.Debug.WriteLine($"   - Durée: {durationHours} heure(s)");
            
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                System.Diagnostics.Debug.WriteLine("❌ Utilisateur non connecté");
                return (false, null, "Utilisateur non connecté");
            }

            System.Diagnostics.Debug.WriteLine($"✅ Utilisateur connecté: {user.name} (ID: {user.id})");

            if (!int.TryParse(lockerId, out int lockerIdInt))
            {
                System.Diagnostics.Debug.WriteLine($"❌ ID casier invalide: {lockerId}");
                return (false, null, "ID casier invalide");
            }

            // Calculer la date de fin et le montant
            var plannedEndAt = DateTime.Now.AddHours(durationHours);
            var plannedEndAtStr = plannedEndAt.ToString("yyyy-MM-dd HH:mm:ss");
            var amountDue = (decimal)durationHours * 2.50m; // 2.50€ par heure
            
            System.Diagnostics.Debug.WriteLine($"   - Fin prévue: {plannedEndAtStr}");
            System.Diagnostics.Debug.WriteLine($"   - Montant: {amountDue:F2}€");

            // Créer la session via l'API
            var (success, message, session) = await _sessionService.CreateSessionAsync(
                user.id, 
                lockerIdInt, 
                "active",
                plannedEndAtStr,
                amountDue,
                "EUR",
                "none");
            
            if (success && session != null)
            {
                System.Diagnostics.Debug.WriteLine($"✅ Session créée avec succès: ID {session.Id}");
                return (true, session.ToLockerSession(), message);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"❌ Échec création session: {message}");
                return (false, null, message ?? "Erreur lors de la création de la session");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Exception création session: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"❌ Stack trace: {ex.StackTrace}");
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
