using SmartLockerApp.DTOs;

namespace SmartLockerApp.Services;

/// <summary>
/// Service d'intégration avec l'API SmartLocker
/// Utilise ApiHttpService pour faire les appels et gère la logique métier
/// </summary>
public class SmartLockerApiService
{
    private readonly ApiHttpService _httpService;

    public SmartLockerApiService()
    {
        _httpService = new ApiHttpService();
    }

    #region Authentification

    /// <summary>
    /// Se connecte à l'API et stocke le token JWT
    /// </summary>
    public async Task<bool> LoginAsync(string username, string password)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"🔐 Tentative login API: {username}");
            
            var request = new LoginRequestDto
            {
                username = username,
                password = password
            };

            var response = await _httpService.PostAsync<LoginRequestDto, LoginResponseDto>("login", request);
            
            if (response != null && !string.IsNullOrEmpty(response.token))
            {
                System.Diagnostics.Debug.WriteLine($"✅ Login réussi, token reçu: {response.token.Substring(0, Math.Min(20, response.token.Length))}...");
                _httpService.SetJwtToken(response.token);
                return true;
            }

            System.Diagnostics.Debug.WriteLine("❌ Login échoué: pas de token dans la réponse");
            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur login: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"❌ Stack: {ex.StackTrace}");
            return false;
        }
    }

    /// <summary>
    /// Se déconnecte (supprime le token)
    /// </summary>
    public void Logout()
    {
        _httpService.ClearJwtToken();
    }

    /// <summary>
    /// Teste si la connexion est valide
    /// </summary>
    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            // Utilise l'endpoint /protected pour tester
            await _httpService.GetAsync<object>("protected");
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Crée un nouvel utilisateur
    /// </summary>
    public async Task<CreateUserResponseDto?> CreateUserAsync(string username, string password, string email, string name, string role = "user")
    {
        try
        {
            var request = new CreateUserRequestDto
            {
                username = username,
                password = password,
                email = email,
                name = name,
                role = role
            };

            var response = await _httpService.PostAsync<CreateUserRequestDto, CreateUserResponseDto>("users", request);
            return response;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur CreateUser: {ex.Message}");
            return new CreateUserResponseDto
            {
                success = false,
                message = $"Erreur lors de la création: {ex.Message}"
            };
        }
    }

    #endregion

    #region Casiers

    /// <summary>
    /// Récupère tous les casiers
    /// </summary>
    public async Task<List<LockerDto>> GetLockersAsync()
    {
        try
        {
            var lockers = await _httpService.GetAsync<List<LockerDto>>("lockers");
            return lockers ?? new List<LockerDto>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur GetLockers: {ex.Message}");
            return new List<LockerDto>();
        }
    }

    /// <summary>
    /// Récupère les casiers disponibles
    /// </summary>
    public async Task<List<LockerDto>> GetAvailableLockersAsync()
    {
        try
        {
            var lockers = await _httpService.GetAsync<List<LockerDto>>("lockers/available");
            return lockers ?? new List<LockerDto>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur GetAvailableLockers: {ex.Message}");
            return new List<LockerDto>();
        }
    }

    /// <summary>
    /// Récupère un casier par son ID
    /// </summary>
    public async Task<LockerDto?> GetLockerAsync(int lockerId)
    {
        try
        {
            return await _httpService.GetAsync<LockerDto>($"lockers/{lockerId}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur GetLocker: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Ouvre un casier (marque l'ouverture)
    /// </summary>
    public async Task<bool> OpenLockerAsync(int lockerId)
    {
        try
        {
            var response = await _httpService.PostAsync<object, OpenLockerResponseDto>($"lockers/{lockerId}/open", new { });
            return response?.success ?? false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur OpenLocker: {ex.Message}");
            return false;
        }
    }

    #endregion
    #region Sessions

    /// <summary>
    /// Démarre une nouvelle session
    /// </summary>
    public async Task<SessionDto?> StartSessionAsync(int userId, int lockerId, DateTime endTime)
    {
        try
        {
            var request = new StartSessionDto
            {
                user_id = userId,
                locker_id = lockerId,
                planned_end_at = endTime
            };

            return await _httpService.PostAsync<StartSessionDto, SessionDto>("sessions", request);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur StartSession: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Crée une nouvelle session de casier dans la BDD
    /// </summary>
    public async Task<CreateSessionResponseDto?> CreateSessionAsync(int userId, int lockerId, DateTime startTime, DateTime endTime, decimal cost)
    {
        try
        {
            var request = new CreateSessionRequestDto
            {
                user_id = userId,
                locker_id = lockerId,
                start_time = startTime.ToString("yyyy-MM-dd HH:mm:ss"),
                end_time = endTime.ToString("yyyy-MM-dd HH:mm:ss"),
                cost = cost,
                status = "active"
            };

            var response = await _httpService.PostAsync<CreateSessionRequestDto, CreateSessionResponseDto>("sessions/create", request);
            return response;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur CreateSession: {ex.Message}");
            return new CreateSessionResponseDto
            {
                success = false,
                message = $"Erreur lors de la création de session: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Clôture une session
    /// </summary>
    public async Task<bool> CloseSessionAsync(int sessionId, string paymentStatus = "paid")
    {
        try
        {
            var request = new CloseSessionDto { payment_status = paymentStatus };
            await _httpService.PostAsync($"sessions/{sessionId}/close", request);
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur CloseSession: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Récupère les sessions actives de l'utilisateur courant
    /// </summary>
    public async Task<List<SessionDto>> GetMyActiveSessionsAsync()
    {
        try
        {
            var sessions = await _httpService.GetAsync<List<SessionDto>>("me/sessions?status=active");
            return sessions ?? new List<SessionDto>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur GetMyActiveSessions: {ex.Message}");
            return new List<SessionDto>();
        }
    }

    /// <summary>
    /// Récupère une session par son ID
    /// </summary>
    public async Task<SessionDto?> GetSessionAsync(int sessionId)
    {
        try
        {
            return await _httpService.GetAsync<SessionDto>($"sessions/{sessionId}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur GetSession: {ex.Message}");
            return null;
        }
    }

    #endregion

    #region Utilisateurs

    /// <summary>
    /// Récupère un utilisateur par son ID
    /// </summary>
    public async Task<UserDto?> GetUserAsync(int userId)
    {
        try
        {
            return await _httpService.GetAsync<UserDto>($"users/{userId}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur GetUser: {ex.Message}");
            return null;
        }
    }

    #endregion

    /// <summary>
    /// Vérifie si on est connecté
    /// </summary>
    public bool IsAuthenticated()
    {
        return _httpService.HasValidToken();
    }

    /// <summary>
    /// Libère les ressources
    /// </summary>
    public void Dispose()
    {
        _httpService?.Dispose();
    }
}
