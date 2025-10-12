using System.ComponentModel;
using SmartLockerApp.Models;

namespace SmartLockerApp.Services;

/// <summary>
/// Service de gestion des casiers et sessions
/// </summary>
public class LockerManagementService : INotifyPropertyChanged
{
    private readonly LocalStorageService _storage;
    private readonly AuthenticationService _auth;
    private const string LockersKey = "lockers";
    private const string SessionsKey = "sessions";
    private const string HistoryKey = "history";

    private List<Locker> _lockers = new();
    private List<LockerSession> _activeSessions = new();
    private List<LockerSession> _sessionHistory = new();
    private Timer? _sessionTimer;

    public List<Locker> Lockers => _lockers;
    
    public List<LockerSession> ActiveSessions
    {
        get
        {
            var currentUserId = _auth.CurrentUser?.Id ?? "";
            DebugLogger.Info($"[ActiveSessions] Filtrage avec UserId: '{currentUserId}'");
            DebugLogger.Info($"[ActiveSessions] Nombre total de sessions: {_activeSessions.Count}");
            
            // Si pas d'utilisateur connecté, retourner toutes les sessions actives
            if (string.IsNullOrEmpty(currentUserId))
            {
                DebugLogger.Warning("[ActiveSessions] AUCUN utilisateur connecté - Retour de TOUTES les sessions");
                return _activeSessions.ToList();
            }
            
            var filtered = _activeSessions.Where(s =>
            {
                var match = CompatibilityService.CompareIds(s.UserId, currentUserId);
                DebugLogger.Info($"  → Session {s.Id}: UserId={s.UserId}, Match={match}");
                return match;
            }).ToList();
            
            DebugLogger.Info($"[ActiveSessions] Sessions filtrées: {filtered.Count}");
            return filtered;
        }
    }
    
    public List<LockerSession> SessionHistory => _sessionHistory.Where(s => CompatibilityService.CompareIds(s.UserId, _auth.CurrentUser?.Id ?? "")).ToList();
    public LockerSession? CurrentActiveSession => ActiveSessions.FirstOrDefault();

    public event PropertyChangedEventHandler? PropertyChanged;

    public LockerManagementService(LocalStorageService storage, AuthenticationService auth)
    {
        _storage = storage;
        _auth = auth;
        InitializeLockers();
        _ = LoadDataAsync();
        StartSessionTimer();
    }

    /// <summary>
    /// Initialise les 2 casiers par défaut
    /// </summary>
    private void InitializeLockers()
    {
        _lockers = new List<Locker>
        {
            new Locker
            {
                Id = 1,
                Name = "Entrée principale",
                Size = LockerSize.Medium,
                Status = CompatibilityService.StatusToString(LockerStatus.Available),
                PricePerHour = 3.50m,
                Features = new List<string> { "Sécurisé", "Surveillance 24/7", "Accès facile" }
            },
            new Locker
            {
                Id = 2,
                Name = "Hall principal",
                Size = LockerSize.Large,
                Status = CompatibilityService.StatusToString(LockerStatus.Available),
                PricePerHour = 5.00m,
                Features = new List<string> { "Sécurisé", "Surveillance 24/7", "Grande capacité" }
            }
        };
    }

    /// <summary>
    /// Charge les données depuis le stockage local
    /// </summary>
    private async Task LoadDataAsync()
    {
        // Toujours utiliser les 2 casiers par défaut
        // Ne pas charger les casiers sauvegardés pour garantir qu'il n'y en ait que 2
        
        _activeSessions = await _storage.LoadAsync<List<LockerSession>>(SessionsKey) ?? new();
        _sessionHistory = await _storage.LoadAsync<List<LockerSession>>(HistoryKey) ?? new();

        // Nettoyer les sessions expirées
        await CleanExpiredSessionsAsync();
        
        // Mettre à jour le statut des casiers en fonction des sessions actives
        UpdateLockersStatus();
        
        OnPropertyChanged(nameof(Lockers));
        OnPropertyChanged(nameof(ActiveSessions));
        OnPropertyChanged(nameof(SessionHistory));
    }

    /// <summary>
    /// Met à jour le statut des casiers en fonction des sessions actives
    /// </summary>
    private void UpdateLockersStatus()
    {
        // Réinitialiser tous les casiers comme disponibles
        foreach (var locker in _lockers)
        {
            locker.Status = CompatibilityService.StatusToString(LockerStatus.Available);
            locker.CurrentSessionId = null;
        }

        // Marquer les casiers occupés selon les sessions actives
        foreach (var session in _activeSessions.Where(s => CompatibilityService.CompareSessionStatus(s.Status, SessionStatus.Active)))
        {
            var locker = _lockers.FirstOrDefault(l => l.Id == session.LockerId);
            if (locker != null)
            {
                locker.Status = CompatibilityService.StatusToString(LockerStatus.Occupied);
                locker.CurrentSessionId = CompatibilityService.IntToStringId(session.Id);
            }
        }
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

            var locker = _lockers.FirstOrDefault(l => CompatibilityService.CompareIds(l.Id, lockerId));
            if (locker == null)
                return (false, "Casier introuvable", null);

            if (!CompatibilityService.CompareStatus(locker.Status, LockerStatus.Available))
                return (false, "Casier non disponible", null);

            // Vérifier si l'utilisateur a déjà une session active
            if (ActiveSessions.Any())
                return (false, "Vous avez déjà une session active", null);

            var session = new LockerSession
            {
                Id = new Random().Next(1000, 9999),
                UserId = CompatibilityService.StringToIntId(_auth.CurrentUser.Id),
                LockerId = CompatibilityService.StringToIntId(lockerId),
                StartedAt = DateTime.Now,
                PlannedEndAt = DateTime.Now.AddHours(durationHours),
                DurationHours = durationHours,
                AmountDue = locker.PricePerHour * durationHours,
                Status = CompatibilityService.SessionStatusToString(SessionStatus.Active),
                Items = items,
                IsLocked = false
            };

            // Mettre à jour le statut du casier
            locker.Status = CompatibilityService.StatusToString(LockerStatus.Occupied);
            locker.CurrentSessionId = CompatibilityService.IntToStringId(session.Id);

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
            var session = _activeSessions.FirstOrDefault(s => CompatibilityService.CompareIds(s.Id, sessionId) && CompatibilityService.CompareIds(s.UserId, _auth.CurrentUser?.Id ?? ""));
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
            var session = _activeSessions.FirstOrDefault(s => CompatibilityService.CompareIds(s.Id, sessionId) && CompatibilityService.CompareIds(s.UserId, _auth.CurrentUser?.Id ?? ""));
            if (session == null)
                return (false, "Session introuvable");

            var locker = _lockers.FirstOrDefault(l => l.Id == session.LockerId);
            if (locker != null)
            {
                locker.Status = CompatibilityService.StatusToString(LockerStatus.Available);
                locker.CurrentSessionId = null;
            }

            session.Status = CompatibilityService.SessionStatusToString(SessionStatus.Completed);
            session.ActualEndTime = DateTime.Now;

            // Calculer le coût final (si terminé plus tôt)
            var actualDuration = (session.ActualEndTime.Value - session.StartTime).TotalHours;
            if (actualDuration < session.DurationHours)
            {
                session.TotalCost = (decimal)actualDuration * (locker?.PricePerHour ?? 0);
            }

            _activeSessions.Remove(session);
            _sessionHistory.Add(session);

            // Mettre à jour le statut des casiers
            UpdateLockersStatus();
            
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
        return _lockers.FirstOrDefault(l => CompatibilityService.CompareIds(l.Id, lockerId));
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
            if (DateTime.Now >= session.EndTime && CompatibilityService.CompareSessionStatus(session.Status, SessionStatus.Active))
            {
                session.Status = CompatibilityService.SessionStatusToString(SessionStatus.Expired);
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
                locker.Status = CompatibilityService.StatusToString(LockerStatus.Available);
                locker.CurrentSessionId = null;
            }

            session.Status = CompatibilityService.SessionStatusToString(SessionStatus.Expired);
            session.ActualEndTime = session.EndTime;

            _activeSessions.Remove(session);
            _sessionHistory.Add(session);
        }

        if (expiredSessions.Any())
        {
            // Mettre à jour le statut des casiers après nettoyage
            UpdateLockersStatus();
            await SaveDataAsync();
        }
    }

    /// <summary>
    /// Sauvegarde les données (sessions seulement, les casiers sont fixes)
    /// </summary>
    private async Task SaveDataAsync()
    {
        // Ne pas sauvegarder les casiers car ils sont toujours les 2 mêmes
        await _storage.SaveAsync(SessionsKey, _activeSessions);
        await _storage.SaveAsync(HistoryKey, _sessionHistory);
    }

    /// <summary>
    /// Obtenir les statistiques utilisateur
    /// </summary>
    public (int TotalSessions, decimal TotalSpent, TimeSpan TotalTime) GetUserStats()
    {
        var sessions = _sessionHistory.Where(s => CompatibilityService.CompareIds(s.UserId, _auth.CurrentUser?.Id ?? "")).ToList();
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
        var session = _activeSessions.FirstOrDefault(s => CompatibilityService.CompareIds(s.Id, sessionId));
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
        var activeSession = _activeSessions.FirstOrDefault(s => CompatibilityService.CompareIds(s.Id, sessionId));
        if (activeSession != null)
        {
            return activeSession;
        }

        // Chercher dans l'historique
        var historySession = _sessionHistory.FirstOrDefault(s => CompatibilityService.CompareIds(s.Id, sessionId));
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
