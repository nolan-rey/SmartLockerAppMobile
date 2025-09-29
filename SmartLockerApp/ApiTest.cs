using SmartLockerApp.Services;
using SmartLockerApp.DTOs;

namespace SmartLockerApp;

/// <summary>
/// Test simple de l'API SmartLocker
/// </summary>
public static class ApiTest
{
    public static async Task<string> RunApiTestAsync()
    {
        var results = new List<string>();
        
        try
        {
            results.Add("=== TEST API SMARTLOCKER ===");
            
            // 1. Test de connexion
            var apiService = new SmartLockerApiService();
            results.Add("✅ Service API créé");
            
            // 2. Test d'authentification
            var loginSuccess = await apiService.LoginAsync("SaintMichel", "ITcampus");
            if (loginSuccess)
            {
                results.Add("✅ Authentification réussie");
            }
            else
            {
                results.Add("❌ Échec de l'authentification");
                return string.Join("\n", results);
            }
            
            // 3. Test de récupération des casiers
            var lockers = await apiService.GetLockersAsync();
            if (lockers != null && lockers.Any())
            {
                results.Add($"✅ {lockers.Count} casiers récupérés");
                foreach (var locker in lockers.Take(3))
                {
                    results.Add($"   - Casier {locker.Id}: {locker.Name} ({locker.Status})");
                }
            }
            else
            {
                results.Add("❌ Aucun casier récupéré");
            }
            
            // 4. Test de récupération des casiers disponibles
            var availableLockers = await apiService.GetAvailableLockersAsync();
            if (availableLockers != null && availableLockers.Any())
            {
                results.Add($"✅ {availableLockers.Count} casiers disponibles");
            }
            else
            {
                results.Add("⚠️ Aucun casier disponible");
            }
            
            // 5. Test de connexion à l'API
            var connectionTest = await apiService.TestConnectionAsync();
            if (connectionTest)
            {
                results.Add("✅ Test de connexion API réussi");
            }
            else
            {
                results.Add("❌ Test de connexion API échoué");
            }
            
            results.Add("=== FIN DU TEST ===");
            
        }
        catch (Exception ex)
        {
            results.Add($"❌ Erreur lors du test: {ex.Message}");
        }
        
        return string.Join("\n", results);
    }
}
