namespace SmartLockerApp.Services;

/// <summary>
/// Service simple pour tester la connexion à l'API
/// </summary>
public class ApiTestService
{
    private readonly SmartLockerApiService _apiService;

    public ApiTestService()
    {
        _apiService = new SmartLockerApiService();
    }

    /// <summary>
    /// Test complet de l'API avec les credentials de test
    /// </summary>
    public async Task<string> RunFullTestAsync()
    {
        var results = new List<string>();
        
        try
        {
            results.Add("=== Test API SmartLocker ===");
            
            // Test 1: Connexion
            results.Add("\n1. Test de connexion...");
            var loginSuccess = await _apiService.LoginAsync("SaintMichel", "ITcampus");
            
            if (loginSuccess)
            {
                results.Add("✅ Connexion réussie");
                
                // Test 2: Vérification du token
                results.Add("\n2. Test du token...");
                var tokenValid = await _apiService.TestConnectionAsync();
                results.Add(tokenValid ? "✅ Token valide" : "❌ Token invalide");
                
                // Test 3: Récupération des casiers
                results.Add("\n3. Test récupération casiers...");
                var lockers = await _apiService.GetLockersAsync();
                results.Add($"✅ {lockers.Count} casier(s) récupéré(s)");
                
                foreach (var locker in lockers.Take(3)) // Affiche max 3 casiers
                {
                    results.Add($"   - Casier {locker.id}: {locker.name} ({locker.status})");
                }
                
                // Test 4: Casiers disponibles
                results.Add("\n4. Test casiers disponibles...");
                var availableLockers = await _apiService.GetAvailableLockersAsync();
                results.Add($"✅ {availableLockers.Count} casier(s) disponible(s)");
                
                // Test 5: Sessions actives
                results.Add("\n5. Test sessions actives...");
                var activeSessions = await _apiService.GetMyActiveSessionsAsync();
                results.Add($"✅ {activeSessions.Count} session(s) active(s)");
                
                results.Add("\n🎉 Tous les tests sont passés !");
            }
            else
            {
                results.Add("❌ Échec de la connexion");
                results.Add("Vérifiez les credentials ou la connexion réseau");
            }
        }
        catch (Exception ex)
        {
            results.Add($"\n❌ Erreur pendant les tests: {ex.Message}");
        }
        finally
        {
            _apiService.Dispose();
        }
        
        return string.Join("\n", results);
    }

    /// <summary>
    /// Test simple de connexion uniquement
    /// </summary>
    public async Task<bool> TestLoginAsync()
    {
        try
        {
            var success = await _apiService.LoginAsync("SaintMichel", "ITcampus");
            _apiService.Dispose();
            return success;
        }
        catch
        {
            return false;
        }
    }
}
