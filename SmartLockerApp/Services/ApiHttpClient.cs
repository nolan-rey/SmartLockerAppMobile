using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SmartLockerApp.Services;

/// <summary>
/// Client HTTP avec injection automatique du Bearer Token JWT
/// </summary>
public class ApiHttpClient
{
    // ✅ IMPORTANT : Le slash final est obligatoire pour que HttpClient combine correctement les URLs
    private const string BASE_URL = "https://reymond.alwaysdata.net/smartLockerApi/";
    
    private readonly HttpClient _httpClient;
    private readonly ApiAuthService _authService;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiHttpClient(ApiAuthService authService)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(BASE_URL),
            Timeout = TimeSpan.FromSeconds(30)
        };

        _authService = authService;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    /// <summary>
    /// Configure les headers avec le Bearer Token JWT
    /// </summary>
    private async Task<bool> ConfigureAuthHeaderAsync()
    {
        var token = await _authService.GetValidTokenAsync();
        
        if (string.IsNullOrEmpty(token))
        {
            System.Diagnostics.Debug.WriteLine("❌ Impossible d'obtenir un token valide");
            return false;
        }

        // Configurer le header Authorization avec Bearer Token
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        System.Diagnostics.Debug.WriteLine($"✅ Header Authorization configuré: Bearer {token.Substring(0, Math.Min(20, token.Length))}...");
        
        return true;
    }

    #region Méthodes HTTP

    /// <summary>
    /// Requête GET avec authentification automatique
    /// </summary>
    public async Task<T?> GetAsync<T>(string endpoint)
    {
        try
        {
            if (!await ConfigureAuthHeaderAsync())
                return default;

            System.Diagnostics.Debug.WriteLine($"📤 GET {endpoint}");

            var response = await _httpClient.GetAsync(endpoint);
            var content = await response.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine($"📥 Response: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erreur: {content}");
                return default;
            }

            return JsonSerializer.Deserialize<T>(content, _jsonOptions);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur GET {endpoint}: {ex.Message}");
            return default;
        }
    }

    /// <summary>
    /// Requête POST avec authentification automatique
    /// </summary>
    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        try
        {
            if (!await ConfigureAuthHeaderAsync())
                return default;

            System.Diagnostics.Debug.WriteLine($"📤 POST {endpoint}");

            var json = JsonSerializer.Serialize(data, _jsonOptions);
            System.Diagnostics.Debug.WriteLine($"📤 Request body JSON: {json}");
            
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine($"📥 Response status: {response.StatusCode} ({(int)response.StatusCode})");
            System.Diagnostics.Debug.WriteLine($"📥 Response body: {responseContent}");

            if (!response.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erreur HTTP: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"❌ Raison: {response.ReasonPhrase}");
                System.Diagnostics.Debug.WriteLine($"❌ Contenu de l'erreur: {responseContent}");
                return default;
            }

            return JsonSerializer.Deserialize<TResponse>(responseContent, _jsonOptions);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur POST {endpoint}: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"❌ Stack trace: {ex.StackTrace}");
            return default;
        }
    }

    /// <summary>
    /// Requête POST sans corps de réponse attendu
    /// </summary>
    public async Task<bool> PostAsync<TRequest>(string endpoint, TRequest data)
    {
        try
        {
            if (!await ConfigureAuthHeaderAsync())
                return false;

            System.Diagnostics.Debug.WriteLine($"📤 POST {endpoint}");

            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content);
            
            System.Diagnostics.Debug.WriteLine($"📥 Response: {response.StatusCode}");

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur POST {endpoint}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Requête PUT avec authentification automatique
    /// </summary>
    public async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        try
        {
            if (!await ConfigureAuthHeaderAsync())
                return default;

            System.Diagnostics.Debug.WriteLine($"📤 PUT {endpoint}");

            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine($"📥 Response: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erreur: {responseContent}");
                return default;
            }

            return JsonSerializer.Deserialize<TResponse>(responseContent, _jsonOptions);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur PUT {endpoint}: {ex.Message}");
            return default;
        }
    }

    /// <summary>
    /// Requête DELETE avec authentification automatique
    /// </summary>
    public async Task<bool> DeleteAsync(string endpoint)
    {
        try
        {
            if (!await ConfigureAuthHeaderAsync())
                return false;

            System.Diagnostics.Debug.WriteLine($"📤 DELETE {endpoint}");

            var response = await _httpClient.DeleteAsync(endpoint);
            
            System.Diagnostics.Debug.WriteLine($"📥 Response: {response.StatusCode}");

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur DELETE {endpoint}: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region Méthodes utilitaires

    /// <summary>
    /// Teste la connexion à l'API
    /// </summary>
    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("🔍 Test de connexion à l'API...");
            
            var token = await _authService.GetValidTokenAsync();
            
            if (string.IsNullOrEmpty(token))
            {
                System.Diagnostics.Debug.WriteLine("❌ Impossible d'obtenir un token");
                return false;
            }

            System.Diagnostics.Debug.WriteLine("✅ Connexion API réussie");
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Test de connexion échoué: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Force le renouvellement du token
    /// </summary>
    public async Task<bool> RefreshTokenAsync()
    {
        await _authService.ClearTokenAsync();
        var token = await _authService.GetValidTokenAsync();
        return !string.IsNullOrEmpty(token);
    }

    #endregion
}
