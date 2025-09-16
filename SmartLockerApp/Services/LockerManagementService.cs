using System.ComponentModel;
using SmartLockerApp.Models;

namespace SmartLockerApp.Services;

/// <summary>
/// Service de gestion des casiers et sessions
/// </summary>
public class LockerManagementService : INotifyPropertyChanged
{
    private static LockerManagementService? _instance;
    public static LockerManagementService Instance => _instance ??= new LockerManagementService();

    private readonly LocalStorageService _storage = LocalStorageService.Instance;
    private readonly AuthenticationService _auth = AuthenticationService.Instance;
    private const string LockersKey = "lockers";
    private const string SessionsKey = "sessions";
    private const string HistoryKey = "history";

    private List<Locker> _lockers = new();
    private List<LockerSession> _activeSessions = new();
    private List<LockerSession> _sessionHistory = new();
    private Timer? _sessionTimer;

    public List<Locker> Lockers => _lockers;
    public List<LockerSession> ActiveSessions => _activeSessions.Where(s => s.UserId == _auth.CurrentUser?.Id).ToList();
    public List<LockerSession> SessionHistory => _sessionHistory.Where(s => s.UserId == _auth.CurrentUser?.Id).ToList();
    public LockerSession? CurrentActiveSession => ActiveSessions.FirstOrDefault();

    public event PropertyChangedEventHandler? PropertyChanged;

    private LockerManagementService()
    {
        InitializeLockers();
        _ = LoadDataAsync();
        StartSessionTimer();
    }

    /// <summary>
    /// Initialise les casiers par défaut
    /// </summary>
    private void InitializeLockers()
    {
        _lockers = new List<Locker>();
        
        // Créer 20 casiers avec différentes tailles
        for (int i = 1; i <= 20; i++)
        {
            var size = i <= 8 ? LockerSize.Small : 
                      i <= 16 ? LockerSize.Medium : LockerSize.Large;
            
            _lockers.Add(new Locker
            {
                Id = $"L{i:D3}",
                Location = $"Zone A - Niveau {(i - 1) / 4 + 1}",
                Size = size,
                Status = LockerStatus.Available,
                PricePerHour = size switch
                {
                    LockerSize.Small => 2.50m,
                    LockerSize.Medium => 3.50m,
                    LockerSize.Large => 5.00m,
                    _ => 2.50m
                },
                Features = new List<string> { "Sécurisé", "Surveillance 24/7" }
            });
        }

        // Quelques casiers occupés pour la démo
        _lockers[2].Status = LockerStatus.Occupied;
        _lockers[7].Status = LockerStatus.Occupied;
        _lockers[12].Status = LockerStatus.Maintenance;
    }

    /// <summary>
    /// Charge les données depuis le stockage local
    /// </summary>
    private async Task LoadDataAsync()
    {
        var savedLockers = await _storage.LoadAsync<List<Locker>>(LockersKey);
        if (savedLockers != null && savedLockers.Count > 0)
            _lockers = savedLockers;

        _activeSessions = await _storage.LoadAsync<List<LockerSession>>(SessionsKey) ?? new();
        _sessionHistory = await _storage.LoadAsync<List<LockerSession>>(HistoryKey) ?? new();

        // Nettoyer les sessions expirées
        await CleanExpiredSessionsAsync();
        OnPropertyChanged(nameof(Lockers));
        OnPropertyChanged(nameof(ActiveSessions));
        OnPropertyChanged(nameof(SessionHistory));
    }

    /// <summary>
    /// Démarre une nouvelle session de casier
    /// </summary>
    public async Task<(bool Success, string Message, LockerSession? Session)> StartSessionAsync(string lockerId, int durationHours, List<string> items)
    {
        try
        {
            if (_auth.CurrentUser == null)
                return (false, "Utilisateur non connecté", null);

            var locker = _lockers.FirstOrDefault(l => l.Id == lockerId);
            if (locker == null)
                return (false, "Casier introuvable", null);

            if (locker.Status != LockerStatus.Available)
                return (false, "Casier non disponible", null);

            // Vérifier si l'utilisateur a déjà une session active
            if (ActiveSessions.Any())
                return (false, "Vous avez déjà une session active", null);

            var session = new LockerSession
            {
                Id = Guid.NewGuid().ToString(),
                UserId = _auth.CurrentUser.Id,
                LockerId = lockerId,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(durationHours),
                DurationHours = durationHours,
                TotalCost = locker.PricePerHour * durationHours,
                Status = SessionStatus.Active,
                Items = items,
                IsLocked = false
            };

            // Mettre à jour le statut du casier
            locker.Status = LockerStatus.Occupied;
            locker.CurrentSessionId = session.Id;

            _activeSessions.Add(session);
            await SaveDataAsync();

            OnPropertyChanged(nameof(Lockers));
            OnPropertyChanged(nameof(ActiveSessions));
            OnPropertyChanged(nameof(CurrentActiveSession));

            return (true, "Session créée avec succès", session);
        }
        catch (Exception ex)
        {
            return (false, $"Erreur: {ex.Message}", null);
        }
    }

    /// <summary>
    /// Verrouille le casier après dépôt des affaires
    /// </summary>
    public async Task<bool> LockLockerAsync(string sessionId)
    {
        try
        {
            var session = _activeSessions.FirstOrDefault(s => s.Id == sessionId && s.UserId == _auth.CurrentUser?.Id);
            if (session == null) return false;

            session.IsLocked = true;
            session.LockedAt = DateTime.Now;
            
            await SaveDataAsync();
            OnPropertyChanged(nameof(ActiveSessions));
            OnPropertyChanged(nameof(CurrentActiveSession));

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Termine une session
    /// </summary>
    public async Task<(bool Success, string Message)> EndSessionAsync(string sessionId)
    {
        try
        {
            var session = _activeSessions.FirstOrDefault(s => s.Id == sessionId && s.UserId == _auth.CurrentUser?.Id);
            if (session == null)
                return (false, "Session introuvable");

            var locker = _lockers.FirstOrDefault(l => l.Id == session.LockerId);
            if (locker != null)
            {
                locker.Status = LockerStatus.Available;
                locker.CurrentSessionId = null;
            }

            session.Status = SessionStatus.Completed;
            session.ActualEndTime = DateTime.Now;

            // Calculer le coût final (si terminé plus tôt)
            var actualDuration = (session.ActualEndTime.Value - session.StartTime).TotalHours;
            if (actualDuration < session.DurationHours)
            {
                session.TotalCost = (decimal)actualDuration * (locker?.PricePerHour ?? 0);
            }

            _activeSessions.Remove(session);
            _sessionHistory.Add(session);

            await SaveDataAsync();

            OnPropertyChanged(nameof(Lockers));
            OnPropertyChanged(nameof(ActiveSessions));
            OnPropertyChanged(nameof(SessionHistory));
            OnPropertyChanged(nameof(CurrentActiveSession));

            return (true, "Session terminée avec succès");
        }
        catch (Exception ex)
        {
            return (false, $"Erreur: {ex.Message}");
        }
    }

    /// <summary>
    /// Obtient les détails d'un casier
    /// </summary>
    public Locker? GetLockerDetails(string lockerId)
    {
        return _lockers.FirstOrDefault(l => l.Id == lockerId);
    }

    /// <summary>
    /// Obtient le temps restant pour une session
    /// </summary>
    public TimeSpan GetRemainingTime(LockerSession session)
    {
        var remaining = session.EndTime - DateTime.Now;
        return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
    }

    /// <summary>
    /// Timer pour mettre à jour les sessions
    /// </summary>
    private void StartSessionTimer()
    {
        _sessionTimer = new Timer(async _ => await UpdateSessionsAsync(), null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
    }

    /// <summary>
    /// Met à jour les sessions actives
    /// </summary>
    private async Task UpdateSessionsAsync()
    {
        bool hasChanges = false;

        foreach (var session in _activeSessions.ToList())
        {
            if (DateTime.Now >= session.EndTime && session.Status == SessionStatus.Active)
            {
                session.Status = SessionStatus.Expired;
                hasChanges = true;
            }
        }

        if (hasChanges)
        {
            await SaveDataAsync();
            OnPropertyChanged(nameof(ActiveSessions));
            OnPropertyChanged(nameof(CurrentActiveSession));
        }
    }

    /// <summary>
    /// Nettoie les sessions expirées
    /// </summary>
    private async Task CleanExpiredSessionsAsync()
    {
        var expiredSessions = _activeSessions.Where(s => DateTime.Now > s.EndTime.AddHours(24)).ToList();
        
        foreach (var session in expiredSessions)
        {
            var locker = _lockers.FirstOrDefault(l => l.Id == session.LockerId);
            if (locker != null)
            {
                locker.Status = LockerStatus.Available;
                locker.CurrentSessionId = null;
            }

            session.Status = SessionStatus.Expired;
            session.ActualEndTime = session.EndTime;

            _activeSessions.Remove(session);
            _sessionHistory.Add(session);
        }

        if (expiredSessions.Any())
        {
            await SaveDataAsync();
        }
    }

    /// <summary>
    /// Sauvegarde les données
    /// </summary>
    private async Task SaveDataAsync()
    {
        await _storage.SaveAsync(LockersKey, _lockers);
        await _storage.SaveAsync(SessionsKey, _activeSessions);
        await _storage.SaveAsync(HistoryKey, _sessionHistory);
    }

    /// <summary>
    /// Obtenir les statistiques utilisateur
    /// </summary>
    public (int TotalSessions, decimal TotalSpent, TimeSpan TotalTime) GetUserStats()
    {
        var sessions = _sessionHistory.Where(s => s.UserId == _auth.CurrentUser.Id).ToList();
        var totalSessions = sessions.Count;
        var totalSpent = sessions.Sum(s => s.TotalCost);
        var totalTime = TimeSpan.FromHours(sessions.Sum(s => (s.EndTime - s.StartTime).TotalHours));
        
        return (totalSessions, totalSpent, totalTime);
    }

    /// <summary>
    /// Mettre à jour les items d'une session
    /// </summary>
    public async Task UpdateSessionItemsAsync(string sessionId, List<string> items)
    {
        var session = _activeSessions.FirstOrDefault(s => s.Id == sessionId);
        if (session != null)
        {
            session.Items = items;
            await SaveDataAsync();
        }
    }

    /// <summary>
    /// Obtenir une session spécifique par ID
    /// </summary>
    public async Task<LockerSession?> GetSessionAsync(string sessionId)
    {
        // Chercher d'abord dans les sessions actives
        var activeSession = _activeSessions.FirstOrDefault(s => s.Id == sessionId);
        if (activeSession != null)
        {
            return activeSession;
        }

        // Chercher dans l'historique
        var historySession = _sessionHistory.FirstOrDefault(s => s.Id == sessionId);
        return historySession;
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void Dispose()
    {
        _sessionTimer?.Dispose();
    }
}
