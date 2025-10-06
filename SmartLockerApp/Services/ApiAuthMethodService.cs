using SmartLockerApp.Models;

namespace SmartLockerApp.Services;

/// <summary>
/// Service de gestion des méthodes d'authentification via l'API
/// Utilise ApiHttpClient avec authentification JWT automatique
/// </summary>
public class ApiAuthMethodService
{
    private static ApiAuthMethodService? _instance;
    public static ApiAuthMethodService Instance => _instance ??= new ApiAuthMethodService();

    private readonly ApiHttpClient _apiClient;

    private ApiAuthMethodService()
    {
        _apiClient = ApiHttpClient.Instance;
    }

    #region GET - Récupération

    /// <summary>
    /// GET /auth_methods - Récupère toutes les méthodes d'authentification
    /// </summary>
    public async Task<List<AuthMethod>?> GetAllAuthMethodsAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("📋 Récupération de toutes les méthodes d'authentification...");
            
            var authMethods = await _apiClient.GetAsync<List<AuthMethod>>("/auth_methods");
            
            if (authMethods != null)
            {
                System.Diagnostics.Debug.WriteLine($"✅ {authMethods.Count} méthodes récupérées");
                foreach (var method in authMethods.Take(5))
                {
                    System.Diagnostics.Debug.WriteLine($"   - Méthode #{method.Id}: User {method.UserId}, Type {method.Type}");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("⚠️ Aucune méthode récupérée");
            }
            
            return authMethods;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur GetAllAuthMethods: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// GET /auth_methods/{id} - Récupère une méthode par ID
    /// </summary>
    public async Task<AuthMethod?> GetAuthMethodByIdAsync(int authMethodId)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"🔍 Récupération méthode ID={authMethodId}...");
            
            var authMethod = await _apiClient.GetAsync<AuthMethod>($"/auth_methods/{authMethodId}");
            
            if (authMethod != null)
            {
                System.Diagnostics.Debug.WriteLine($"✅ Méthode trouvée: User {authMethod.UserId}, Type {authMethod.Type}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("⚠️ Méthode non trouvée");
            }
            
            return authMethod;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur GetAuthMethodById: {ex.Message}");
            return null;
        }
    }

    #endregion

    #region POST - Création

    /// <summary>
    /// POST /auth_methods - Crée une nouvelle méthode d'authentification
    /// Body: {"user_id": 1, "type": "rfid", "credential_value": "RFID-TEST-0001"}
    /// </summary>
    public async Task<(bool Success, string Message, AuthMethod? AuthMethod)> CreateAuthMethodAsync(
        int userId,
        string type,
        string credentialValue)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"➕ Création méthode: User {userId}, Type {type}...");

            var authMethodData = new
            {
                user_id = userId,
                type = type,
                credential_value = credentialValue
            };

            var response = await _apiClient.PostAsync<object, SuccessResponse>("/auth_methods", authMethodData);

            if (response?.success == true)
            {
                System.Diagnostics.Debug.WriteLine($"✅ Méthode créée");
                
                // Récupérer la méthode créée
                var allMethods = await GetAllAuthMethodsAsync();
                var createdMethod = allMethods?
                    .Where(m => m.UserId == userId && m.Type == type && m.CredentialValue == credentialValue)
                    .OrderByDescending(m => m.Id)
                    .FirstOrDefault();
                
                return (true, "Méthode créée avec succès", createdMethod);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("❌ Échec création méthode");
                return (false, "Échec de la création de la méthode", null);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur CreateAuthMethod: {ex.Message}");
            return (false, $"Erreur: {ex.Message}", null);
        }
    }

    /// <summary>
    /// Crée une méthode RFID
    /// </summary>
    public async Task<(bool Success, string Message, AuthMethod? AuthMethod)> CreateRfidMethodAsync(
        int userId,
        string rfidValue)
    {
        return await CreateAuthMethodAsync(userId, "rfid", rfidValue);
    }

    /// <summary>
    /// Crée une méthode Fingerprint
    /// </summary>
    public async Task<(bool Success, string Message, AuthMethod? AuthMethod)> CreateFingerprintMethodAsync(
        int userId,
        string fingerprintValue)
    {
        return await CreateAuthMethodAsync(userId, "fingerprint", fingerprintValue);
    }

    #endregion

    #region PUT - Mise à jour

    /// <summary>
    /// PUT /auth_methods/{id} - Met à jour une méthode d'authentification
    /// Body: {"type": "fingerprint", "credential_value": "FP-XYZ"}
    /// </summary>
    public async Task<(bool Success, string Message)> UpdateAuthMethodAsync(
        int authMethodId,
        string? type = null,
        string? credentialValue = null)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"✏️ Mise à jour méthode ID={authMethodId}...");

            var updateData = new
            {
                type = type,
                credential_value = credentialValue
            };

            var response = await _apiClient.PutAsync<object, SuccessResponse>($"/auth_methods/{authMethodId}", updateData);

            if (response?.success == true)
            {
                System.Diagnostics.Debug.WriteLine($"✅ Méthode mise à jour");
                return (true, "Méthode mise à jour avec succès");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("❌ Échec mise à jour méthode");
                return (false, "Échec de la mise à jour de la méthode");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur UpdateAuthMethod: {ex.Message}");
            return (false, $"Erreur: {ex.Message}");
        }
    }

    #endregion

    #region DELETE - Suppression

    /// <summary>
    /// DELETE /auth_methods/{id} - Supprime une méthode d'authentification
    /// </summary>
    public async Task<(bool Success, string Message)> DeleteAuthMethodAsync(int authMethodId)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"🗑️ Suppression méthode ID={authMethodId}...");

            var success = await _apiClient.DeleteAsync($"/auth_methods/{authMethodId}");

            if (success)
            {
                System.Diagnostics.Debug.WriteLine("✅ Méthode supprimée");
                return (true, "Méthode supprimée avec succès");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("❌ Échec suppression méthode");
                return (false, "Échec de la suppression de la méthode");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur DeleteAuthMethod: {ex.Message}");
            return (false, $"Erreur: {ex.Message}");
        }
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Récupère les méthodes d'un utilisateur spécifique
    /// </summary>
    public async Task<List<AuthMethod>?> GetUserAuthMethodsAsync(int userId)
    {
        try
        {
            var allMethods = await GetAllAuthMethodsAsync();
            
            if (allMethods == null)
                return null;

            return allMethods.Where(m => m.UserId == userId).ToList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur GetUserAuthMethods: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Récupère les méthodes par type
    /// </summary>
    public async Task<List<AuthMethod>?> GetAuthMethodsByTypeAsync(string type)
    {
        try
        {
            var allMethods = await GetAllAuthMethodsAsync();
            
            if (allMethods == null)
                return null;

            return allMethods.Where(m => m.Type.Equals(type, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur GetAuthMethodsByType: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Vérifie si un utilisateur a une méthode d'authentification
    /// </summary>
    public async Task<bool> UserHasAuthMethodAsync(int userId, string type)
    {
        try
        {
            var userMethods = await GetUserAuthMethodsAsync(userId);
            return userMethods?.Any(m => m.Type.Equals(type, StringComparison.OrdinalIgnoreCase)) ?? false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur UserHasAuthMethod: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region Tests

    /// <summary>
    /// Teste toutes les opérations CRUD
    /// </summary>
    public async Task TestAllOperationsAsync()
    {
        System.Diagnostics.Debug.WriteLine("\n=== TEST DES OPÉRATIONS CRUD AUTH METHODS ===\n");

        // 1. GET ALL
        System.Diagnostics.Debug.WriteLine("1️⃣ Test GET /auth_methods");
        var allMethods = await GetAllAuthMethodsAsync();
        if (allMethods != null)
        {
            System.Diagnostics.Debug.WriteLine($"   ✅ {allMethods.Count} méthodes récupérées");
        }

        await Task.Delay(500);

        // 2. GET BY ID
        System.Diagnostics.Debug.WriteLine("\n2️⃣ Test GET /auth_methods/1");
        var method = await GetAuthMethodByIdAsync(1);
        if (method != null)
        {
            System.Diagnostics.Debug.WriteLine($"   ✅ Méthode: Type {method.Type}, Credential {method.CredentialValue}");
        }

        await Task.Delay(500);

        // 3. CREATE RFID
        System.Diagnostics.Debug.WriteLine("\n3️⃣ Test POST /auth_methods (RFID)");
        var (createSuccess, createMsg, newMethod) = await CreateRfidMethodAsync(1, "RFID-TEST-9999");
        System.Diagnostics.Debug.WriteLine($"   {(createSuccess ? "✅" : "❌")} {createMsg}");

        await Task.Delay(500);

        if (newMethod != null)
        {
            // 4. UPDATE
            System.Diagnostics.Debug.WriteLine("\n4️⃣ Test PUT /auth_methods/{id}");
            var (updateSuccess, updateMsg) = await UpdateAuthMethodAsync(
                newMethod.Id,
                credentialValue: "RFID-TEST-8888"
            );
            System.Diagnostics.Debug.WriteLine($"   {(updateSuccess ? "✅" : "❌")} {updateMsg}");

            await Task.Delay(500);

            // 5. DELETE
            System.Diagnostics.Debug.WriteLine("\n5️⃣ Test DELETE /auth_methods/{id}");
            var (deleteSuccess, deleteMsg) = await DeleteAuthMethodAsync(newMethod.Id);
            System.Diagnostics.Debug.WriteLine($"   {(deleteSuccess ? "✅" : "❌")} {deleteMsg}");
        }

        await Task.Delay(500);

        // 6. GET USER METHODS
        System.Diagnostics.Debug.WriteLine("\n6️⃣ Test Get User Methods");
        var userMethods = await GetUserAuthMethodsAsync(1);
        if (userMethods != null)
        {
            System.Diagnostics.Debug.WriteLine($"   ✅ {userMethods.Count} méthode(s) pour l'utilisateur");
        }

        System.Diagnostics.Debug.WriteLine("\n=== FIN DES TESTS ===\n");
    }

    #endregion

    #region DTOs

    /// <summary>
    /// DTO pour les réponses de succès simples
    /// </summary>
    private class SuccessResponse
    {
        public bool success { get; set; }
    }

    #endregion
}
