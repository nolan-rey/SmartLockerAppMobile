using SmartLockerApp.Models;

namespace SmartLockerApp.Services;

/// <summary>
/// Service de gestion des liaisons Session/Authentification via l'API
/// </summary>
public class ApiSessionAuthService
{
    private static ApiSessionAuthService? _instance;
    public static ApiSessionAuthService Instance => _instance ??= new ApiSessionAuthService();

    private readonly ApiHttpClient _apiClient;

    private ApiSessionAuthService()
    {
        _apiClient = ApiHttpClient.Instance;
    }

    /// <summary>
    /// GET /session_auth - Récupère toutes les liaisons
    /// </summary>
    public async Task<List<SessionAuth>?> GetAllSessionAuthsAsync()
    {
        try
        {
            var sessionAuths = await _apiClient.GetAsync<List<SessionAuth>>("/session_auth");
            System.Diagnostics.Debug.WriteLine($"✅ {sessionAuths?.Count ?? 0} liaisons récupérées");
            return sessionAuths;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur GetAllSessionAuths: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// GET /session_auth/{id} - Récupère une liaison par ID
    /// </summary>
    public async Task<SessionAuth?> GetSessionAuthByIdAsync(int sessionAuthId)
    {
        try
        {
            return await _apiClient.GetAsync<SessionAuth>($"/session_auth/{sessionAuthId}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur GetSessionAuthById: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// POST /session_auth - Crée une nouvelle liaison
    /// </summary>
    public async Task<(bool Success, string Message)> CreateSessionAuthAsync(
        int sessionId,
        int authMethodId)
    {
        try
        {
            var sessionAuthData = new
            {
                session_id = sessionId,
                auth_method_id = authMethodId
            };

            var response = await _apiClient.PostAsync<object, SuccessResponse>("/session_auth", sessionAuthData);

            if (response?.success == true)
            {
                System.Diagnostics.Debug.WriteLine($"✅ Liaison créée");
                return (true, "Liaison créée avec succès");
            }
            
            return (false, "Échec de la création de la liaison");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur CreateSessionAuth: {ex.Message}");
            return (false, $"Erreur: {ex.Message}");
        }
    }

    /// <summary>
    /// DELETE /session_auth/{id} - Supprime une liaison
    /// </summary>
    public async Task<(bool Success, string Message)> DeleteSessionAuthAsync(int sessionAuthId)
    {
        try
        {
            var success = await _apiClient.DeleteAsync($"/session_auth/{sessionAuthId}");
            return success 
                ? (true, "Liaison supprimée avec succès") 
                : (false, "Échec de la suppression de la liaison");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur DeleteSessionAuth: {ex.Message}");
            return (false, $"Erreur: {ex.Message}");
        }
    }

    /// <summary>
    /// Récupère les liaisons d'une session
    /// </summary>
    public async Task<List<SessionAuth>?> GetSessionAuthsBySessionIdAsync(int sessionId)
    {
        try
        {
            var all = await GetAllSessionAuthsAsync();
            return all?.Where(sa => sa.SessionId == sessionId).ToList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur: {ex.Message}");
            return null;
        }
    }

    private class SuccessResponse
    {
        public bool success { get; set; }
    }
}
