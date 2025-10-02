using System.Text;
using System.Text.Json;

namespace SmartLockerApp.Services;

/// <summary>
/// Service HTTP simple pour communiquer avec l'API SmartLocker
/// Gère automatiquement l'authentification JWT et la sérialisation JSON
/// </summary>
public class ApiHttpService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private string? _jwtToken;

    public ApiHttpService()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("https://reymond.alwaysdata.net/smartLockerApi/");
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
        
        // Configuration JSON simple
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    /// <summary>
    /// Définit le token JWT pour les requêtes authentifiées
    /// </summary>
    public void SetJwtToken(string token)
    {
        _jwtToken = token;
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
    }

    /// <summary>
    /// Supprime le token JWT
    /// </summary>
    public void ClearJwtToken()
    {
        _jwtToken = null;
        _httpClient.DefaultRequestHeaders.Clear();
    }

    /// <summary>
    /// Requête GET simple
    /// </summary>
    public async Task<T?> GetAsync<T>(string endpoint)
    {
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"GET {endpoint} failed: {response.StatusCode}");
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erreur GET {endpoint}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Requête POST avec données JSON
    /// </summary>
    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            System.Diagnostics.Debug.WriteLine($"POST {endpoint} - Request: {json}");
            
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            // Log des headers
            var hasAuthHeader = _httpClient.DefaultRequestHeaders.Contains("Authorization");
            System.Diagnostics.Debug.WriteLine($"POST {endpoint} - Has Auth Header: {hasAuthHeader}");
            
            var response = await _httpClient.PostAsync(endpoint, content);
            
            var responseJson = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"POST {endpoint} - Status: {response.StatusCode}");
            System.Diagnostics.Debug.WriteLine($"POST {endpoint} - Response: {responseJson}");
            
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"POST {endpoint} failed: {response.StatusCode} - {responseJson}");
            }

            return JsonSerializer.Deserialize<TResponse>(responseJson, _jsonOptions);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"POST {endpoint} - Exception: {ex.Message}");
            throw new Exception($"Erreur POST {endpoint}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Requête POST sans réponse attendue
    /// </summary>
    public async Task PostAsync<TRequest>(string endpoint, TRequest data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync(endpoint, content);
            
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"POST {endpoint} failed: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Erreur POST {endpoint}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Requête PUT avec données JSON
    /// </summary>
    public async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync(endpoint, content);
            
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"PUT {endpoint} failed: {response.StatusCode}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(responseJson, _jsonOptions);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erreur PUT {endpoint}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Requête PUT sans réponse attendue
    /// </summary>
    public async Task PutAsync<TRequest>(string endpoint, TRequest data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync(endpoint, content);
            
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"PUT {endpoint} failed: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Erreur PUT {endpoint}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Requête DELETE
    /// </summary>
    public async Task DeleteAsync(string endpoint)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(endpoint);
            
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"DELETE {endpoint} failed: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Erreur DELETE {endpoint}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Vérifie si on a un token JWT valide
    /// </summary>
    public bool HasValidToken()
    {
        return !string.IsNullOrEmpty(_jwtToken);
    }

    /// <summary>
    /// Libère les ressources
    /// </summary>
    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}
