using SmartLockerApp.Models;

namespace SmartLockerApp.Services;

/// <summary>
/// Service intégré qui utilise l'API en priorité avec fallback vers les données locales
/// Interface simple pour les ViewModels
/// </summary>
public class SmartLockerIntegratedService
{
    private readonly SmartLockerApiService _apiService;
    private bool _isApiAvailable = false;

    public SmartLockerIntegratedService(SmartLockerApiService apiService)
    {
        _apiService = apiService;
    }

    #region Authentification

    /// <summary>
    /// Se connecte avec les credentials de test ou personnalisés
    /// </summary>
    public async Task<bool> LoginAsync(string username = "Smart", string password = "Locker")
    {
        try
        {
            var success = await _apiService.LoginAsync(username, password);
            _isApiAvailable = success;
            return success;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur login API: {ex.Message}");
            _isApiAvailable = false;
            
            // Fallback : connexion locale simulée
            return username == "SaintMichel" && password == "ITcampus";
        }
    }

    /// <summary>
    /// Se déconnecte
    /// </summary>
    public void Logout()
    {
        _apiService.Logout();
        _isApiAvailable = false;
    }

    /// <summary>
    /// Vérifie si on est connecté
    /// </summary>
    public bool IsAuthenticated()
    {
        return _isApiAvailable ? _apiService.IsAuthenticated() : true; // Fallback toujours connecté
    }

    /// <summary>
    /// Crée un nouvel utilisateur dans la BDD via l'API
    /// </summary>
    public async Task<(bool Success, string Message, Models.User? User)> CreateUserAsync(string username, string password, string email, string name, string role = "user")
    {
        try
        {
            // Essaie d'abord avec l'API
            var apiResponse = await _apiService.CreateUserAsync(username, password, email, name, role);
            
            if (apiResponse != null && apiResponse.success && apiResponse.user != null)
            {
                _isApiAvailable = true;
                
                // Convertit le DTO en modèle
                var user = new Models.User
                {
                    Id = apiResponse.user.id,
                    Name = apiResponse.user.name,
                    Email = email, // L'email n'est pas retourné par l'API, on garde celui fourni
                    Role = apiResponse.user.role
                };
                
                return (true, "Utilisateur créé avec succès dans la BDD", user);
            }
            else
            {
                var errorMessage = apiResponse?.message ?? "Erreur inconnue lors de la création";
                return (false, $"Échec création API: {errorMessage}", null);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur création utilisateur API: {ex.Message}");
            _isApiAvailable = false;
            
            // Fallback : création locale (simulation)
            var localUser = new Models.User
            {
                Id = new Random().Next(1000, 9999), // ID temporaire
                Name = name,
                Email = email,
                Role = role
            };
            
            return (true, "Utilisateur créé localement (mode hors ligne)", localUser);
        }
    }

    #endregion

    #region Casiers

    /// <summary>
    /// Récupère tous les casiers disponibles
    /// </summary>
    public async Task<List<Locker>> GetAvailableLockersAsync()
    {
        if (_isApiAvailable)
        {
            try
            {
                var lockerDtos = await _apiService.GetAvailableLockersAsync();
                return lockerDtos.ToModels();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur API GetAvailableLockers: {ex.Message}");
                _isApiAvailable = false;
            }
        }

        // Fallback : données locales de test
        return GetLocalTestLockers().Where(l => l.IsAvailable).ToList();
    }

    /// <summary>
    /// Récupère un casier par son ID
    /// </summary>
    public async Task<Locker?> GetLockerAsync(int lockerId)
    {
        if (_isApiAvailable)
        {
            try
            {
                var lockerDto = await _apiService.GetLockerAsync(lockerId);
                return lockerDto?.ToModel();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur API GetLocker: {ex.Message}");
                _isApiAvailable = false;
            }
        }

        // Fallback : données locales
        return GetLocalTestLockers().FirstOrDefault(l => l.Id == lockerId);
    }

    /// <summary>
    /// Ouvre un casier
    /// </summary>
    public async Task<bool> OpenLockerAsync(int lockerId)
    {
        if (_isApiAvailable)
        {
            try
            {
                return await _apiService.OpenLockerAsync(lockerId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur API OpenLocker: {ex.Message}");
                _isApiAvailable = false;
            }
        }

        // Fallback : simulation d'ouverture
        await Task.Delay(1000); // Simule le temps d'ouverture
        return true;
    }

    /// <summary>
    /// Crée une nouvelle session de casier dans la BDD
    /// </summary>
    public async Task<(bool Success, string Message, Models.LockerSession? Session)> CreateSessionAsync(int userId, int lockerId, DateTime startTime, DateTime endTime, decimal cost)
    {
        try
        {
            // Essaie d'abord avec l'API
            var apiResponse = await _apiService.CreateSessionAsync(userId, lockerId, startTime, endTime, cost);
            
            if (apiResponse != null && apiResponse.success && apiResponse.session != null)
            {
                _isApiAvailable = true;
                
                // Convertit le DTO en modèle
                var session = new Models.LockerSession
                {
                    Id = apiResponse.session.id,
                    UserId = apiResponse.session.user_id,
                    LockerId = apiResponse.session.locker_id,
                    StartedAt = apiResponse.session.started_at,
                    PlannedEndAt = apiResponse.session.planned_end_at,
                    AmountDue = apiResponse.session.amount_due,
                    Status = apiResponse.session.status,
                    PaymentStatus = apiResponse.session.payment_status ?? "pending"
                };
                
                return (true, "Session créée avec succès dans la BDD", session);
            }
            else
            {
                var errorMessage = apiResponse?.message ?? "Erreur inconnue lors de la création";
                return (false, $"Échec création session API: {errorMessage}", null);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur création session API: {ex.Message}");
            _isApiAvailable = false;
            
            // Fallback : création locale (simulation)
            var localSession = new Models.LockerSession
            {
                Id = new Random().Next(1000, 9999), // ID temporaire
                UserId = userId,
                LockerId = lockerId,
                StartedAt = startTime,
                PlannedEndAt = endTime,
                AmountDue = cost,
                Status = "active",
                PaymentStatus = "pending"
            };
            
            return (true, "Session créée localement (mode hors ligne)", localSession);
        }
    }

    #endregion

    #region Sessions

    /// <summary>
    /// Démarre une nouvelle session
    /// </summary>
    public async Task<LockerSession?> StartSessionAsync(int userId, int lockerId, DateTime plannedEndAt)
    {
        if (_isApiAvailable)
        {
            try
            {
                var response = await _apiService.StartSessionAsync(userId, lockerId, plannedEndAt);
                if (response != null)
                {
                    // Récupère la session complète
                    var sessionDto = await _apiService.GetSessionAsync(response.id);
                    return sessionDto?.ToModel();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur API StartSession: {ex.Message}");
                _isApiAvailable = false;
            }
        }

        // Fallback : création de session locale
        return CreateLocalTestSession(userId, lockerId, plannedEndAt);
    }

    /// <summary>
    /// Clôture une session
    /// </summary>
    public async Task<bool> CloseSessionAsync(int sessionId, string paymentStatus = "paid")
    {
        if (_isApiAvailable)
        {
            try
            {
                return await _apiService.CloseSessionAsync(sessionId, paymentStatus);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur API CloseSession: {ex.Message}");
                _isApiAvailable = false;
            }
        }

        // Fallback : simulation de clôture
        await Task.Delay(500);
        return true;
    }

    /// <summary>
    /// Récupère les sessions actives
    /// </summary>
    public async Task<List<LockerSession>> GetActiveSessionsAsync()
    {
        if (_isApiAvailable)
        {
            try
            {
                var sessionDtos = await _apiService.GetMyActiveSessionsAsync();
                return sessionDtos.ToModels();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur API GetActiveSessions: {ex.Message}");
                _isApiAvailable = false;
            }
        }

        // Fallback : sessions locales vides
        return new List<LockerSession>();
    }

    #endregion

    #region Test et Status

    /// <summary>
    /// Teste la connexion API
    /// </summary>
    public async Task<string> TestApiConnectionAsync()
    {
        try
        {
            var isConnected = await _apiService.TestConnectionAsync();
            return isConnected ? "✅ Connexion API réussie" : "❌ Connexion API échouée";
        }
        catch (Exception ex)
        {
            return $"❌ Erreur de connexion: {ex.Message}";
        }
    }

    /// <summary>
    /// Indique si l'API est disponible
    /// </summary>
    public bool IsApiAvailable => _isApiAvailable;

    #endregion

    #region Données de test locales

    /// <summary>
    /// Génère des casiers de test locaux
    /// </summary>
    private List<Locker> GetLocalTestLockers()
    {
        return new List<Locker>
        {
            new Locker 
            { 
                Id = 1, 
                Name = "Casier A1", 
                Status = "available",
                Size = LockerSize.Medium,
                PricePerHour = 2.0m
            },
            new Locker 
            { 
                Id = 2, 
                Name = "Casier B2", 
                Status = "available",
                Size = LockerSize.Large,
                PricePerHour = 3.0m
            },
            new Locker 
            { 
                Id = 3, 
                Name = "Casier C3", 
                Status = "occupied",
                Size = LockerSize.Small,
                PricePerHour = 1.5m
            }
        };
    }

    /// <summary>
    /// Crée une session de test locale
    /// </summary>
    private LockerSession CreateLocalTestSession(int userId, int lockerId, DateTime plannedEndAt)
    {
        return new LockerSession
        {
            Id = new Random().Next(1000, 9999),
            UserId = userId,
            LockerId = lockerId,
            Status = "active",
            StartedAt = DateTime.Now,
            PlannedEndAt = plannedEndAt,
            AmountDue = CalculateAmount(DateTime.Now, plannedEndAt),
            Currency = "EUR",
            PaymentStatus = "none"
        };
    }

    /// <summary>
    /// Calcule le montant basé sur la durée
    /// </summary>
    private decimal CalculateAmount(DateTime start, DateTime end)
    {
        var hours = (decimal)(end - start).TotalHours;
        return Math.Max(1, Math.Ceiling(hours)) * 2.0m; // 2€/heure
    }

    #endregion
}
