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

    // Propriétés observables avec génération automatique des notifications
    public User? CurrentUser => _auth.CurrentUser != null ? new User
    {
        Id = _auth.CurrentUser.Id,
        FirstName = _auth.CurrentUser.FirstName,
        LastName = _auth.CurrentUser.LastName,
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
    /// Connexion utilisateur
    /// </summary>
    public async Task<bool> LoginAsync(string email, string password)
    {
        var (success, message) = await _auth.LoginAsync(email, password);
        if (success)
        {
            // Notifier les changements d'état après connexion
            NotifyStateChanged();
        }
        return success;
    }

    /// <summary>
    /// Création de compte
    /// </summary>
    public async Task<(bool Success, string Message)> CreateAccountAsync(string email, string password, string firstName, string lastName)
    {
        return await _auth.CreateAccountAsync(email, password, firstName, lastName);
    }

    /// <summary>
    /// Déconnexion
    /// </summary>
    public async Task LogoutAsync()
    {
        await _auth.LogoutAsync();
        NotifyStateChanged();
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

        var (success, message) = await _lockerService.EndSessionAsync(ActiveSession.Id);
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
