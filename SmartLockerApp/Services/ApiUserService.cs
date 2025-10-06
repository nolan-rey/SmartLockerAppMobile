using SmartLockerApp.Models;

namespace SmartLockerApp.Services;

/// <summary>
/// Service de gestion des utilisateurs via l'API
/// Utilise ApiHttpClient avec authentification JWT automatique
/// </summary>
public class ApiUserService
{
    private readonly ApiHttpClient _apiClient;

    public ApiUserService(ApiHttpClient apiClient)
    {
        _apiClient = apiClient;
    }

    #region GET - Récupération

    /// <summary>
    /// GET /users - Récupère tous les utilisateurs
    /// </summary>
    public async Task<List<User>?> GetAllUsersAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("📋 Récupération de tous les utilisateurs...");
            
            var users = await _apiClient.GetAsync<List<User>>("/users");
            
            if (users != null)
            {
                System.Diagnostics.Debug.WriteLine($"✅ {users.Count} utilisateurs récupérés");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("⚠️ Aucun utilisateur récupéré");
            }
            
            return users;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur GetAllUsers: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// GET /users/{id} - Récupère un utilisateur par ID
    /// </summary>
    public async Task<User?> GetUserByIdAsync(int userId)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"🔍 Récupération utilisateur ID={userId}...");
            
            var user = await _apiClient.GetAsync<User>($"/users/{userId}");
            
            if (user != null)
            {
                System.Diagnostics.Debug.WriteLine($"✅ Utilisateur trouvé: {user.name}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("⚠️ Utilisateur non trouvé");
            }
            
            return user;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur GetUserById: {ex.Message}");
            return null;
        }
    }

    #endregion

    #region POST - Création

    /// <summary>
    /// POST /users - Crée un nouvel utilisateur
    /// </summary>
    public async Task<(bool Success, string Message, User? User)> CreateUserAsync(
        string name,
        string email,
        string passwordHash,
        string role = "user")
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"➕ Création utilisateur: {name} ({email})...");

            var userData = new
            {
                name = name,
                email = email,
                password_hash = passwordHash,
                role = role
            };

            var createdUser = await _apiClient.PostAsync<object, User>("/users", userData);

            if (createdUser != null)
            {
                System.Diagnostics.Debug.WriteLine($"✅ Utilisateur créé avec ID={createdUser.id}");
                return (true, "Utilisateur créé avec succès", createdUser);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("❌ Échec création utilisateur");
                return (false, "Échec de la création de l'utilisateur", null);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur CreateUser: {ex.Message}");
            return (false, $"Erreur: {ex.Message}", null);
        }
    }

    #endregion

    #region PUT - Mise à jour

    /// <summary>
    /// PUT /users/{id} - Met à jour un utilisateur
    /// </summary>
    public async Task<(bool Success, string Message, User? User)> UpdateUserAsync(
        int userId,
        string? name = null,
        string? email = null,
        string? role = null)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"✏️ Mise à jour utilisateur ID={userId}...");

            var updateData = new
            {
                name = name,
                email = email,
                role = role
            };

            var updatedUser = await _apiClient.PutAsync<object, User>($"/users/{userId}", updateData);

            if (updatedUser != null)
            {
                System.Diagnostics.Debug.WriteLine($"✅ Utilisateur mis à jour: {updatedUser.name}");
                return (true, "Utilisateur mis à jour avec succès", updatedUser);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("❌ Échec mise à jour utilisateur");
                return (false, "Échec de la mise à jour de l'utilisateur", null);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur UpdateUser: {ex.Message}");
            return (false, $"Erreur: {ex.Message}", null);
        }
    }

    #endregion

    #region DELETE - Suppression

    /// <summary>
    /// DELETE /users/{id} - Supprime un utilisateur
    /// </summary>
    public async Task<(bool Success, string Message)> DeleteUserAsync(int userId)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"🗑️ Suppression utilisateur ID={userId}...");

            var success = await _apiClient.DeleteAsync($"/users/{userId}");

            if (success)
            {
                System.Diagnostics.Debug.WriteLine("✅ Utilisateur supprimé");
                return (true, "Utilisateur supprimé avec succès");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("❌ Échec suppression utilisateur");
                return (false, "Échec de la suppression de l'utilisateur");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur DeleteUser: {ex.Message}");
            return (false, $"Erreur: {ex.Message}");
        }
    }

    #endregion

    #region Tests

    /// <summary>
    /// Teste toutes les opérations CRUD
    /// </summary>
    public async Task TestAllOperationsAsync()
    {
        System.Diagnostics.Debug.WriteLine("\n=== TEST DES OPÉRATIONS CRUD USERS ===\n");

        // 1. GET ALL
        System.Diagnostics.Debug.WriteLine("1️⃣ Test GET /users");
        var allUsers = await GetAllUsersAsync();
        if (allUsers != null)
        {
            System.Diagnostics.Debug.WriteLine($"   ✅ {allUsers.Count} utilisateurs récupérés");
        }

        await Task.Delay(500);

        // 2. GET BY ID
        System.Diagnostics.Debug.WriteLine("\n2️⃣ Test GET /users/1");
        var user = await GetUserByIdAsync(1);
        if (user != null)
        {
            System.Diagnostics.Debug.WriteLine($"   ✅ Utilisateur: {user.name} ({user.email})");
        }

        await Task.Delay(500);

        // 3. CREATE
        System.Diagnostics.Debug.WriteLine("\n3️⃣ Test POST /users");
        var (createSuccess, createMsg, newUser) = await CreateUserAsync(
            "Test User",
            "test@smartlocker.com",
            "hashedpassword123",
            "user"
        );
        System.Diagnostics.Debug.WriteLine($"   {(createSuccess ? "✅" : "❌")} {createMsg}");

        await Task.Delay(500);

        if (newUser != null)
        {
            // 4. UPDATE
            System.Diagnostics.Debug.WriteLine("\n4️⃣ Test PUT /users/{id}");
            var (updateSuccess, updateMsg, updatedUser) = await UpdateUserAsync(
                newUser.id,
                name: "Test User Updated"
            );
            System.Diagnostics.Debug.WriteLine($"   {(updateSuccess ? "✅" : "❌")} {updateMsg}");

            await Task.Delay(500);

            // 5. DELETE
            System.Diagnostics.Debug.WriteLine("\n5️⃣ Test DELETE /users/{id}");
            var (deleteSuccess, deleteMsg) = await DeleteUserAsync(newUser.id);
            System.Diagnostics.Debug.WriteLine($"   {(deleteSuccess ? "✅" : "❌")} {deleteMsg}");
        }

        System.Diagnostics.Debug.WriteLine("\n=== FIN DES TESTS ===\n");
    }

    #endregion
}
