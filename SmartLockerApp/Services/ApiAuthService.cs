using System.Text;
using System.Text.Json;

namespace SmartLockerApp.Services;

/// <summary>
/// Service d'authentification API avec gestion du token JWT
/// </summary>
public class ApiAuthService
{
    // ✅ IMPORTANT : Le slash final est obligatoire pour que HttpClient combine correctement les URLs
    private const string BASE_URL = "https://reymond.alwaysdata.net/smartLockerApi/";
    private const string ADMIN_USERNAME = "Smart";
    private const string ADMIN_PASSWORD = "Locker";
    private const int TOKEN_VALIDITY_MINUTES = 60; // 1 heure

    private readonly HttpClient _httpClient;
    private string? _currentToken;
    private DateTime _tokenExpiration = DateTime.MinValue;

    public ApiAuthService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(BASE_URL),
            Timeout = TimeSpan.FromSeconds(30)
        };
    }

    /// <summary>
    /// Récupère le token JWT actuel (le renouvelle si expiré)
    /// </summary>
    public async Task<string?> GetValidTokenAsync()
    {
        // Si le token est encore valide (avec marge de 5 minutes)
        if (!string.IsNullOrEmpty(_currentToken) && DateTime.Now < _tokenExpiration.AddMinutes(-5))
        {
            System.Diagnostics.Debug.WriteLine($"✅ Token JWT valide, expire dans {(_tokenExpiration - DateTime.Now).TotalMinutes:F1} min");
            return _currentToken;
        }

        // Sinon, obtenir un nouveau token
        System.Diagnostics.Debug.WriteLine("🔄 Token expiré ou inexistant, obtention d'un nouveau token...");
        return await LoginAsync(ADMIN_USERNAME, ADMIN_PASSWORD);
    }

    /// <summary>
    /// Authentification et récupération du token JWT
    /// POST /login avec {"username": "Smart", "password": "Locker"}
    /// </summary>
    public async Task<string?> LoginAsync(string username, string password)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"🔐 Tentative login API: {username}");

            var loginData = new
            {
                username = username,
                password = password
            };

            var json = JsonSerializer.Serialize(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var fullUrl = $"{_httpClient.BaseAddress}login";
            System.Diagnostics.Debug.WriteLine($"📤 POST URL complète: {fullUrl}");
            System.Diagnostics.Debug.WriteLine($"📤 Request body: {json}");
            System.Diagnostics.Debug.WriteLine($"📤 Content-Type: application/json");

            var response = await _httpClient.PostAsync("login", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine($"📥 Response status: {response.StatusCode} ({(int)response.StatusCode})");
            System.Diagnostics.Debug.WriteLine($"📥 Response body: {responseBody}");
            System.Diagnostics.Debug.WriteLine($"📥 Response headers: {response.Headers}");

            if (!response.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Login échoué: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"❌ Raison: {response.ReasonPhrase}");
                return null;
            }

            // Parser la réponse JSON pour extraire le token
            var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseBody);

            if (loginResponse?.token != null)
            {
                _currentToken = loginResponse.token;
                _tokenExpiration = DateTime.Now.AddMinutes(TOKEN_VALIDITY_MINUTES);

                System.Diagnostics.Debug.WriteLine($"✅ Token JWT obtenu avec succès");
                System.Diagnostics.Debug.WriteLine($"✅ Token (début): {_currentToken.Substring(0, Math.Min(30, _currentToken.Length))}...");
                System.Diagnostics.Debug.WriteLine($"✅ Expire le: {_tokenExpiration:HH:mm:ss}");

                // Sauvegarder le token de manière sécurisée
                await SaveTokenSecurelyAsync(_currentToken);

                return _currentToken;
            }

            System.Diagnostics.Debug.WriteLine("❌ Pas de token dans la réponse");
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur login API: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"❌ Stack: {ex.StackTrace}");
            return null;
        }
    }

    /// <summary>
    /// Sauvegarde le token de manière sécurisée
    /// </summary>
    private async Task SaveTokenSecurelyAsync(string token)
    {
        try
        {
            await SecureStorage.SetAsync("jwt_token", token);
            await SecureStorage.SetAsync("jwt_token_expiration", _tokenExpiration.ToString("o"));
            System.Diagnostics.Debug.WriteLine("✅ Token sauvegardé dans SecureStorage");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"⚠️ Impossible de sauvegarder le token: {ex.Message}");
        }
    }

    /// <summary>
    /// Charge le token depuis le stockage sécurisé
    /// </summary>
    public async Task<bool> LoadTokenFromStorageAsync()
    {
        try
        {
            var token = await SecureStorage.GetAsync("jwt_token");
            var expirationStr = await SecureStorage.GetAsync("jwt_token_expiration");

            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(expirationStr))
            {
                var expiration = DateTime.Parse(expirationStr);

                // Vérifier si le token est encore valide
                if (DateTime.Now < expiration.AddMinutes(-5))
                {
                    _currentToken = token;
                    _tokenExpiration = expiration;
                    System.Diagnostics.Debug.WriteLine($"✅ Token chargé depuis SecureStorage, valide jusqu'à {expiration:HH:mm:ss}");
                    return true;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ Token expiré dans SecureStorage");
                    await ClearTokenAsync();
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"⚠️ Impossible de charger le token: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Supprime le token (déconnexion)
    /// </summary>
    public async Task ClearTokenAsync()
    {
        _currentToken = null;
        _tokenExpiration = DateTime.MinValue;
        
        try
        {
            SecureStorage.Remove("jwt_token");
            SecureStorage.Remove("jwt_token_expiration");
            System.Diagnostics.Debug.WriteLine("✅ Token supprimé");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"⚠️ Erreur suppression token: {ex.Message}");
        }
    }

    /// <summary>
    /// Vérifie si un token valide existe
    /// </summary>
    public bool HasValidToken()
    {
        return !string.IsNullOrEmpty(_currentToken) && DateTime.Now < _tokenExpiration.AddMinutes(-5);
    }

    /// <summary>
    /// Obtient le temps restant avant expiration du token
    /// </summary>
    public TimeSpan GetTokenRemainingTime()
    {
        if (!HasValidToken())
            return TimeSpan.Zero;

        return _tokenExpiration - DateTime.Now;
    }

    #region DTOs

    /// <summary>
    /// DTO pour la réponse de login
    /// </summary>
    private class LoginResponse
    {
        public string? token { get; set; }
    }

    #endregion
}
