using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SmartLockerApp.Services;

/// <summary>
/// Service optimisé pour la sauvegarde locale des données avec CommunityToolkit
/// </summary>
public partial class LocalStorageService : ObservableObject
{
    private static LocalStorageService? _instance;
    public static LocalStorageService Instance => _instance ??= new LocalStorageService();

    private readonly string _dataPath;

    // Options JSON réutilisables pour de meilleures performances
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private LocalStorageService()
    {
        _dataPath = Path.Combine(FileSystem.AppDataDirectory, "SmartLockerData");
        Directory.CreateDirectory(_dataPath);
    }

    /// <summary>
    /// Obtient le chemin complet d'un fichier
    /// </summary>
    private string GetFilePath(string key) => Path.Combine(_dataPath, $"{key}.json");

    /// <summary>
    /// Sauvegarde un objet en JSON de manière asynchrone
    /// </summary>
    public async Task<bool> SaveAsync<T>(string key, T data)
    {
        try
        {
            var filePath = GetFilePath(key);
            var json = JsonSerializer.Serialize(data, JsonOptions);
            await File.WriteAllTextAsync(filePath, json);
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur sauvegarde {key}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Charge un objet depuis JSON de manière asynchrone
    /// </summary>
    public async Task<T?> LoadAsync<T>(string key)
    {
        try
        {
            var filePath = GetFilePath(key);
            if (!File.Exists(filePath))
                return default;

            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<T>(json, JsonOptions);
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
    public bool Delete(string key)
    {
        try
        {
            var filePath = GetFilePath(key);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur suppression {key}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Vérifie si un fichier existe
    /// </summary>
    public bool Exists(string key) => File.Exists(GetFilePath(key));

    /// <summary>
    /// Obtient la taille d'un fichier en octets
    /// </summary>
    public long GetFileSize(string key)
    {
        try
        {
            var filePath = GetFilePath(key);
            return File.Exists(filePath) ? new FileInfo(filePath).Length : 0;
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// Obtient la date de dernière modification d'un fichier
    /// </summary>
    public DateTime? GetLastModified(string key)
    {
        try
        {
            var filePath = GetFilePath(key);
            return File.Exists(filePath) ? File.GetLastWriteTime(filePath) : null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Liste tous les fichiers de données disponibles
    /// </summary>
    public List<string> GetAllKeys()
    {
        try
        {
            return Directory.GetFiles(_dataPath, "*.json")
                .Select(Path.GetFileNameWithoutExtension)
                .Where(name => !string.IsNullOrEmpty(name))
                .ToList()!;
        }
        catch
        {
            return new List<string>();
        }
    }

    /// <summary>
    /// Vide complètement le stockage local
    /// </summary>
    public Task<bool> ClearAllAsync()
    {
        try
        {
            var files = Directory.GetFiles(_dataPath, "*.json");
            foreach (var file in files)
            {
                File.Delete(file);
            }
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur vidage stockage: {ex.Message}");
            return Task.FromResult(false);
        }
    }

    /// <summary>
    /// Sauvegarde avec une valeur par défaut si la clé n'existe pas
    /// </summary>
    public async Task<T> LoadOrDefaultAsync<T>(string key, T defaultValue)
    {
        var result = await LoadAsync<T>(key);
        return result ?? defaultValue;
    }

    /// <summary>
    /// Sauvegarde seulement si la clé n'existe pas déjà
    /// </summary>
    public async Task<bool> SaveIfNotExistsAsync<T>(string key, T data)
    {
        if (Exists(key))
            return false;
        
        return await SaveAsync(key, data);
    }
}
