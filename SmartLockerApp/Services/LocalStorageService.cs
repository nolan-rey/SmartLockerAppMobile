using System.Text.Json;

namespace SmartLockerApp.Services;

/// <summary>
/// Service pour la sauvegarde locale des données
/// </summary>
public class LocalStorageService
{
    private static LocalStorageService? _instance;
    public static LocalStorageService Instance => _instance ??= new LocalStorageService();

    private readonly string _dataPath;

    private LocalStorageService()
    {
        _dataPath = Path.Combine(FileSystem.AppDataDirectory, "SmartLockerData");
        Directory.CreateDirectory(_dataPath);
    }

    /// <summary>
    /// Sauvegarde un objet en JSON
    /// </summary>
    public async Task SaveAsync<T>(string key, T data)
    {
        try
        {
            var filePath = Path.Combine(_dataPath, $"{key}.json");
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            await File.WriteAllTextAsync(filePath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur sauvegarde {key}: {ex.Message}");
        }
    }

    /// <summary>
    /// Charge un objet depuis JSON
    /// </summary>
    public async Task<T?> LoadAsync<T>(string key)
    {
        try
        {
            var filePath = Path.Combine(_dataPath, $"{key}.json");
            if (!File.Exists(filePath))
                return default;

            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<T>(json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur chargement {key}: {ex.Message}");
            return default;
        }
    }

    /// <summary>
    /// Supprime un fichier de données
    /// </summary>
    public void Delete(string key)
    {
        try
        {
            var filePath = Path.Combine(_dataPath, $"{key}.json");
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur suppression {key}: {ex.Message}");
        }
    }

    /// <summary>
    /// Vérifie si un fichier existe
    /// </summary>
    public bool Exists(string key)
    {
        var filePath = Path.Combine(_dataPath, $"{key}.json");
        return File.Exists(filePath);
    }
}
