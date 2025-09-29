using CommunityToolkit.Mvvm.ComponentModel;
using SmartLockerApp.Models;

namespace SmartLockerApp.Services;

/// <summary>
/// Service global de gestion de l'état de l'application utilisant CommunityToolkit.Mvvm
/// </summary>
public partial class AppStateService : ObservableObject
{
    private static AppStateService? _instance;
    public static AppStateService Instance => _instance ??= new AppStateService();

    private readonly AuthenticationService _auth = AuthenticationService.Instance;
    private readonly LockerManagementService _lockerService = LockerManagementService.Instance;
    private readonly SmartLockerIntegratedService _smartLockerService = new SmartLockerIntegratedService(new SmartLockerApiService());

    // Propriétés observables avec génération automatique des notifications
    public User? CurrentUser => _auth.CurrentUser != null ? new User
    {
        Id = int.TryParse(_auth.CurrentUser.Id, out int id) ? id : 0,
        Name = $"{_auth.CurrentUser.FirstName} {_auth.CurrentUser.LastName}".Trim(),
        Email = _auth.CurrentUser.Email
    } : null;

    public List<Locker> Lockers => _lockerService.Lockers;
    public List<LockerSession> SessionHistory => _lockerService.SessionHistory;
    public LockerSession? ActiveSession => _lockerService.CurrentActiveSession;
    public bool IsLoggedIn => _auth.IsAuthenticated;

    private AppStateService()
    {
        // S'abonner aux changements des services pour propager les notifications
        _lockerService.PropertyChanged += (s, e) => OnPropertyChanged(e?.PropertyName);
    }

    /// <summary>
    /// Connexion utilisateur via l'API SmartLocker
    /// </summary>
    public async Task<bool> LoginAsync(string email, string password)
    {
        try
        {
            // Essayer d'abord avec l'API
            var username = email.Split('@')[0]; // Utiliser la partie avant @ comme username
            var apiSuccess = await _smartLockerService.LoginAsync(username, password);
            
            if (apiSuccess)
            {
                // Connexion API réussie, créer/mettre à jour l'utilisateur local
                var localResult = await _auth.LoginAsync(email, password);
                NotifyStateChanged();
                return true;
            }
            else
            {
                // Fallback : connexion locale
                var (success, message) = await _auth.LoginAsync(email, password);
                if (success)
                {
                    NotifyStateChanged();
                }
                return success;
            }
        }
        catch (Exception ex)
        {
            // En cas d'erreur API, fallback vers connexion locale
            System.Diagnostics.Debug.WriteLine($"Erreur connexion API: {ex.Message}");
            var (success, message) = await _auth.LoginAsync(email, password);
            if (success)
            {
                NotifyStateChanged();
            }
            return success;
        }
    }

    /// <summary>
    /// Création de compte via l'API SmartLocker
    /// </summary>
    public async Task<(bool Success, string Message)> CreateAccountAsync(string email, string password, string firstName, string lastName)
    {
        try
        {
            // Générer un nom d'utilisateur basé sur l'email
            var username = email.Split('@')[0];
            var fullName = $"{firstName} {lastName}".Trim();
            
            // Créer l'utilisateur via l'API
            var (success, message, user) = await _smartLockerService.CreateUserAsync(username, password, email, fullName);
            
            if (success && user != null)
            {
                // Créer aussi localement pour compatibilité avec l'ancien système
                var localResult = await _auth.CreateAccountAsync(email, password, firstName, lastName);
                
                // Notifier les changements d'état
                NotifyStateChanged();
                
                return (true, $"Compte créé avec succès dans la BDD ! {message}");
            }
            else
            {
                // Fallback : création locale uniquement
                var localResult = await _auth.CreateAccountAsync(email, password, firstName, lastName);
                if (localResult.Success)
                {
                    return (true, $"Compte créé localement (API indisponible). {message}");
                }
                else
                {
                    return (false, $"Échec de création: {message}");
                }
            }
        }
        catch (Exception ex)
        {
            // En cas d'erreur, fallback vers création locale
            var localResult = await _auth.CreateAccountAsync(email, password, firstName, lastName);
            if (localResult.Success)
            {
                return (true, "Compte créé localement (erreur API)");
            }
            else
            {
                return (false, $"Erreur lors de la création: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Déconnexion
    /// </summary>
    public async Task LogoutAsync()
    {
        await _auth.LogoutAsync();
        _smartLockerService.Logout();
        NotifyStateChanged();
    }

    /// <summary>
    /// Crée une nouvelle session de casier dans la BDD
    /// </summary>
    public async Task<(bool Success, string Message, LockerSession? Session)> CreateSessionAsync(string lockerId, DateTime startTime, DateTime endTime, decimal cost)
    {
        try
        {
            if (CurrentUser == null)
            {
                return (false, "Utilisateur non connecté", null);
            }

            // Convertir l'ID du casier (A1 -> 1, B2 -> 2, etc.)
            var numericLockerId = CompatibilityService.StringToIntId(lockerId);
            
            // Créer la session via l'API
            var (success, message, session) = await _smartLockerService.CreateSessionAsync(
                CurrentUser.Id, 
                numericLockerId, 
                startTime, 
                endTime, 
                cost
            );
            
            if (success && session != null)
            {
                // Ajouter aussi à l'ancien système pour compatibilité
                var durationHours = (int)(endTime - startTime).TotalHours;
                await _lockerService.StartSessionAsync(lockerId, durationHours, new List<string>());
                
                // Notifier les changements
                NotifyStateChanged();
                
                return (true, $"Session créée avec succès dans la BDD ! {message}", session);
            }
            else
            {
                // Fallback : création locale uniquement
                var durationHours = (int)(endTime - startTime).TotalHours;
                var localResult = await _lockerService.StartSessionAsync(lockerId, durationHours, new List<string>());
                if (localResult.Success && localResult.Session != null)
                {
                    NotifyStateChanged();
                    return (true, $"Session créée localement (API indisponible). {message}", localResult.Session);
                }
                else
                {
                    return (false, $"Échec de création de session: {message}", null);
                }
            }
        }
        catch (Exception ex)
        {
            // En cas d'erreur, fallback vers création locale
            var durationHours = (int)(endTime - startTime).TotalHours;
            var localResult = await _lockerService.StartSessionAsync(lockerId, durationHours, new List<string>());
            if (localResult.Success && localResult.Session != null)
            {
                NotifyStateChanged();
                return (true, "Session créée localement (erreur API)", localResult.Session);
            }
            else
            {
                return (false, $"Erreur lors de la création de session: {ex.Message}", null);
            }
        }
    }

    /// <summary>
    /// Démarrer une nouvelle session
    /// </summary>
    public async Task<bool> StartSessionAsync(string lockerId, int durationHours)
    {
        var (success, message, session) = await _lockerService.StartSessionAsync(lockerId, durationHours, new List<string>());
        if (success)
        {
            NotifyStateChanged();
        }
        return success;
    }

    /// <summary>
    /// Démarrer une session avec items
    /// </summary>
    public async Task<(bool Success, string Message, LockerSession? Session)> StartSessionWithItemsAsync(string lockerId, int durationHours, List<string> items)
    {
        var result = await _lockerService.StartSessionAsync(lockerId, durationHours, items);
        if (result.Success)
        {
            NotifyStateChanged();
        }
        return result;
    }

    /// <summary>
    /// Verrouiller le casier
    /// </summary>
    public async Task<bool> LockLockerAsync(string sessionId)
    {
        var success = await _lockerService.LockLockerAsync(sessionId);
        if (success)
        {
            OnPropertyChanged(nameof(ActiveSession));
        }
        return success;
    }

    /// <summary>
    /// Terminer la session active
    /// </summary>
    public async Task<bool> EndSessionAsync()
    {
        if (ActiveSession == null) return false;

        var (success, message) = await _lockerService.EndSessionAsync(CompatibilityService.IntToStringId(ActiveSession.Id));
        if (success)
        {
            NotifyStateChanged();
        }
        return success;
    }

    /// <summary>
    /// Terminer une session spécifique
    /// </summary>
    public async Task<(bool Success, string Message)> EndSessionAsync(string sessionId)
    {
        var result = await _lockerService.EndSessionAsync(sessionId);
        if (result.Success)
        {
            NotifyStateChanged();
        }
        return result;
    }

    /// <summary>
    /// Obtenir les détails d'un casier
    /// </summary>
    public Locker? GetLockerDetails(string lockerId)
    {
        return _lockerService.GetLockerDetails(lockerId);
    }

    /// <summary>
    /// Obtenir le temps restant pour la session active
    /// </summary>
    public TimeSpan GetRemainingTime()
    {
        if (ActiveSession == null) return TimeSpan.Zero;
        return _lockerService.GetRemainingTime(ActiveSession);
    }

    /// <summary>
    /// Obtenir les statistiques utilisateur
    /// </summary>
    public (int TotalSessions, decimal TotalSpent, TimeSpan TotalTime) GetUserStats()
    {
        return _lockerService.GetUserStats();
    }

    /// <summary>
    /// Mettre à jour les items d'une session
    /// </summary>
    public async Task UpdateSessionItemsAsync(string sessionId, List<string> items)
    {
        await _lockerService.UpdateSessionItemsAsync(sessionId, items);
        OnPropertyChanged(nameof(ActiveSession));
    }

    /// <summary>
    /// Obtenir une session spécifique par ID
    /// </summary>
    public async Task<LockerSession?> GetSessionAsync(string sessionId)
    {
        return await _lockerService.GetSessionAsync(sessionId);
    }

    /// <summary>
    /// Vérifier si l'utilisateur est connecté
    /// </summary>
    public bool IsUserLoggedIn()
    {
        return IsLoggedIn && CurrentUser != null;
    }

    /// <summary>
    /// Notifie tous les changements d'état principaux
    /// </summary>
    private void NotifyStateChanged()
    {
        OnPropertyChanged(nameof(CurrentUser));
        OnPropertyChanged(nameof(IsLoggedIn));
        OnPropertyChanged(nameof(ActiveSession));
        OnPropertyChanged(nameof(SessionHistory));
        OnPropertyChanged(nameof(Lockers));
    }
}
