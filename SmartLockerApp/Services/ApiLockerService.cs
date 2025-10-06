using SmartLockerApp.Models;

namespace SmartLockerApp.Services;

/// <summary>
/// Service de gestion des casiers via l'API
/// Utilise ApiHttpClient avec authentification JWT automatique
/// </summary>
public class ApiLockerService
{
    private static ApiLockerService? _instance;
    public static ApiLockerService Instance => _instance ??= new ApiLockerService();

    private readonly ApiHttpClient _apiClient;

    private ApiLockerService()
    {
        _apiClient = ApiHttpClient.Instance;
    }

    #region GET - Récupération

    /// <summary>
    /// GET /lockers - Récupère tous les casiers
    /// </summary>
    public async Task<List<Locker>?> GetAllLockersAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("📋 Récupération de tous les casiers...");
            
            var lockers = await _apiClient.GetAsync<List<Locker>>("/lockers");
            
            if (lockers != null)
            {
                System.Diagnostics.Debug.WriteLine($"✅ {lockers.Count} casiers récupérés");
                foreach (var locker in lockers)
                {
                    System.Diagnostics.Debug.WriteLine($"   - {locker.Name} ({locker.Status})");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("⚠️ Aucun casier récupéré");
            }
            
            return lockers;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur GetAllLockers: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// GET /lockers/{id} - Récupère un casier par ID
    /// </summary>
    public async Task<Locker?> GetLockerByIdAsync(int lockerId)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"🔍 Récupération casier ID={lockerId}...");
            
            var locker = await _apiClient.GetAsync<Locker>($"/lockers/{lockerId}");
            
            if (locker != null)
            {
                System.Diagnostics.Debug.WriteLine($"✅ Casier trouvé: {locker.Name} ({locker.Status})");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("⚠️ Casier non trouvé");
            }
            
            return locker;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur GetLockerById: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// GET /lockers/available - Récupère tous les casiers disponibles
    /// </summary>
    public async Task<List<Locker>?> GetAvailableLockersAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("📋 Récupération des casiers disponibles...");
            
            var lockers = await _apiClient.GetAsync<List<Locker>>("/lockers/available");
            
            if (lockers != null)
            {
                System.Diagnostics.Debug.WriteLine($"✅ {lockers.Count} casiers disponibles");
                foreach (var locker in lockers)
                {
                    System.Diagnostics.Debug.WriteLine($"   - {locker.Name}");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("⚠️ Aucun casier disponible");
            }
            
            return lockers;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur GetAvailableLockers: {ex.Message}");
            return null;
        }
    }

    #endregion

    #region POST - Création

    /// <summary>
    /// POST /lockers - Crée un nouveau casier
    /// Body: {"name": "Casier A3", "status": "available"}
    /// </summary>
    public async Task<(bool Success, string Message)> CreateLockerAsync(
        string name,
        string status = "available")
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"➕ Création casier: {name} ({status})...");

            var lockerData = new
            {
                name = name,
                status = status
            };

            var response = await _apiClient.PostAsync<object, SuccessResponse>("/lockers", lockerData);

            if (response?.success == true)
            {
                System.Diagnostics.Debug.WriteLine($"✅ Casier créé: {name}");
                return (true, "Casier créé avec succès");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("❌ Échec création casier");
                return (false, "Échec de la création du casier");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur CreateLocker: {ex.Message}");
            return (false, $"Erreur: {ex.Message}");
        }
    }

    #endregion

    #region PUT - Mise à jour

    /// <summary>
    /// PUT /lockers/{id} - Met à jour un casier
    /// Body: {"name": "Casier A1", "status": "available"}
    /// </summary>
    public async Task<(bool Success, string Message)> UpdateLockerAsync(
        int lockerId,
        string? name = null,
        string? status = null)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"✏️ Mise à jour casier ID={lockerId}...");

            var updateData = new
            {
                name = name,
                status = status
            };

            var response = await _apiClient.PutAsync<object, SuccessResponse>($"/lockers/{lockerId}", updateData);

            if (response?.success == true)
            {
                System.Diagnostics.Debug.WriteLine($"✅ Casier mis à jour: {name ?? "N/A"}");
                return (true, "Casier mis à jour avec succès");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("❌ Échec mise à jour casier");
                return (false, "Échec de la mise à jour du casier");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur UpdateLocker: {ex.Message}");
            return (false, $"Erreur: {ex.Message}");
        }
    }

    /// <summary>
    /// Change le statut d'un casier
    /// </summary>
    public async Task<(bool Success, string Message)> SetLockerStatusAsync(
        int lockerId,
        string status)
    {
        return await UpdateLockerAsync(lockerId, status: status);
    }

    /// <summary>
    /// Marque un casier comme disponible
    /// </summary>
    public async Task<(bool Success, string Message)> SetLockerAvailableAsync(int lockerId)
    {
        return await SetLockerStatusAsync(lockerId, "available");
    }

    /// <summary>
    /// Marque un casier comme occupé
    /// </summary>
    public async Task<(bool Success, string Message)> SetLockerOccupiedAsync(int lockerId)
    {
        return await SetLockerStatusAsync(lockerId, "occupied");
    }

    /// <summary>
    /// Marque un casier en maintenance
    /// </summary>
    public async Task<(bool Success, string Message)> SetLockerMaintenanceAsync(int lockerId)
    {
        return await SetLockerStatusAsync(lockerId, "maintenance");
    }

    #endregion

    #region DELETE - Suppression

    /// <summary>
    /// DELETE /lockers/{id} - Supprime un casier
    /// </summary>
    public async Task<(bool Success, string Message)> DeleteLockerAsync(int lockerId)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"🗑️ Suppression casier ID={lockerId}...");

            var success = await _apiClient.DeleteAsync($"/lockers/{lockerId}");

            if (success)
            {
                System.Diagnostics.Debug.WriteLine("✅ Casier supprimé");
                return (true, "Casier supprimé avec succès");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("❌ Échec suppression casier");
                return (false, "Échec de la suppression du casier");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur DeleteLocker: {ex.Message}");
            return (false, $"Erreur: {ex.Message}");
        }
    }

    #endregion

    #region Actions spéciales

    /// <summary>
    /// POST /lockers/{id}/open - Ouvre un casier
    /// </summary>
    public async Task<(bool Success, string Message)> OpenLockerAsync(int lockerId)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"🔓 Ouverture casier ID={lockerId}...");

            var success = await _apiClient.PostAsync($"/lockers/{lockerId}/open", new { });

            if (success)
            {
                System.Diagnostics.Debug.WriteLine("✅ Casier ouvert");
                return (true, "Casier ouvert avec succès");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("❌ Échec ouverture casier");
                return (false, "Échec de l'ouverture du casier");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur OpenLocker: {ex.Message}");
            return (false, $"Erreur: {ex.Message}");
        }
    }

    #endregion

    #region Helpers & Statistiques

    /// <summary>
    /// Récupère le nombre de casiers par statut
    /// </summary>
    public async Task<Dictionary<string, int>> GetLockerStatisticsAsync()
    {
        try
        {
            var lockers = await GetAllLockersAsync();
            
            if (lockers == null)
                return new Dictionary<string, int>();

            var stats = lockers
                .GroupBy(l => l.Status)
                .ToDictionary(g => g.Key, g => g.Count());

            System.Diagnostics.Debug.WriteLine("📊 Statistiques casiers:");
            foreach (var stat in stats)
            {
                System.Diagnostics.Debug.WriteLine($"   - {stat.Key}: {stat.Value}");
            }

            return stats;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur GetLockerStatistics: {ex.Message}");
            return new Dictionary<string, int>();
        }
    }

    /// <summary>
    /// Recherche des casiers par nom
    /// </summary>
    public async Task<List<Locker>?> SearchLockersByNameAsync(string searchTerm)
    {
        try
        {
            var allLockers = await GetAllLockersAsync();
            
            if (allLockers == null)
                return null;

            return allLockers
                .Where(l => l.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur SearchLockersByName: {ex.Message}");
            return null;
        }
    }

    #endregion

    #region Tests

    /// <summary>
    /// Teste toutes les opérations CRUD et actions
    /// </summary>
    public async Task TestAllOperationsAsync()
    {
        System.Diagnostics.Debug.WriteLine("\n=== TEST DES OPÉRATIONS CRUD LOCKERS ===\n");

        // 1. GET ALL
        System.Diagnostics.Debug.WriteLine("1️⃣ Test GET /lockers");
        var allLockers = await GetAllLockersAsync();
        if (allLockers != null)
        {
            System.Diagnostics.Debug.WriteLine($"   ✅ {allLockers.Count} casiers récupérés");
        }

        await Task.Delay(500);

        // 2. GET AVAILABLE
        System.Diagnostics.Debug.WriteLine("\n2️⃣ Test GET /lockers/available");
        var availableLockers = await GetAvailableLockersAsync();
        if (availableLockers != null)
        {
            System.Diagnostics.Debug.WriteLine($"   ✅ {availableLockers.Count} casiers disponibles");
        }

        await Task.Delay(500);

        // 3. GET BY ID
        System.Diagnostics.Debug.WriteLine("\n3️⃣ Test GET /lockers/1");
        var locker = await GetLockerByIdAsync(1);
        if (locker != null)
        {
            System.Diagnostics.Debug.WriteLine($"   ✅ Casier: {locker.Name} ({locker.Status})");
        }

        await Task.Delay(500);

        // 4. CREATE
        System.Diagnostics.Debug.WriteLine("\n4️⃣ Test POST /lockers");
        var (createSuccess, createMsg) = await CreateLockerAsync("Casier Test", "available");
        System.Diagnostics.Debug.WriteLine($"   {(createSuccess ? "✅" : "❌")} {createMsg}");

        await Task.Delay(500);

        // 5. UPDATE
        System.Diagnostics.Debug.WriteLine("\n5️⃣ Test PUT /lockers/2");
        var (updateSuccess, updateMsg) = await UpdateLockerAsync(2, name: "Casier A1 Updated");
        System.Diagnostics.Debug.WriteLine($"   {(updateSuccess ? "✅" : "❌")} {updateMsg}");

        await Task.Delay(500);

        // 6. OPEN
        System.Diagnostics.Debug.WriteLine("\n6️⃣ Test POST /lockers/1/open");
        var (openSuccess, openMsg) = await OpenLockerAsync(1);
        System.Diagnostics.Debug.WriteLine($"   {(openSuccess ? "✅" : "❌")} {openMsg}");

        await Task.Delay(500);

        // 7. STATISTICS
        System.Diagnostics.Debug.WriteLine("\n7️⃣ Test Statistiques");
        var stats = await GetLockerStatisticsAsync();
        System.Diagnostics.Debug.WriteLine($"   ✅ {stats.Count} types de statuts");

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
