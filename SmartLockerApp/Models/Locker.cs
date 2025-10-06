using System.Text.Json.Serialization;

namespace SmartLockerApp.Models;

/// <summary>
/// Modèle représentant un casier
/// Compatible avec l'API SmartLocker
/// </summary>
public class Locker
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("status")]
    public string Status { get; set; } = "available";
    
    [JsonPropertyName("last_opened_at")]
    public string? LastOpenedAt { get; set; }
    
    [JsonPropertyName("updated_at")]
    public string? UpdatedAt { get; set; }
    
    // Propriété calculée pour la date de dernière ouverture
    public DateTime? LastOpenedAtDate => string.IsNullOrEmpty(LastOpenedAt) || LastOpenedAt == "null" 
        ? null 
        : DateTime.TryParse(LastOpenedAt, out var date) ? date : null;
    
    // Propriété calculée pour la date de mise à jour
    public DateTime? UpdatedAtDate => string.IsNullOrEmpty(UpdatedAt) 
        ? null 
        : DateTime.TryParse(UpdatedAt, out var date) ? date : null;
    
    // Propriétés calculées pour compatibilité avec l'ancien modèle
    public string Location => Name;
    public LockerSize Size { get; set; } = LockerSize.Medium;
    public decimal PricePerHour { get; set; } = 2.0m;
    public List<string> Features { get; set; } = new();
    public string? CurrentSessionId { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
    
    // Propriétés calculées pour l'affichage
    public LockerStatus StatusEnum 
    { 
        get => Status.ToLower() switch
        {
            "available" => LockerStatus.Available,
            "occupied" => LockerStatus.Occupied,
            "maintenance" => LockerStatus.Maintenance,
            "out_of_order" => LockerStatus.OutOfOrder,
            _ => LockerStatus.Available
        };
        set => Status = value switch
        {
            LockerStatus.Available => "available",
            LockerStatus.Occupied => "occupied",
            LockerStatus.Maintenance => "maintenance",
            LockerStatus.OutOfOrder => "out_of_order",
            _ => "available"
        };
    }
    
    public bool IsAvailable => Status == "available";
}

/// <summary>
/// Taille du casier
/// </summary>
public enum LockerSize
{
    Small,
    Medium,
    Large
}

/// <summary>
/// Statut du casier
/// </summary>
public enum LockerStatus
{
    Available,
    Occupied,
    Maintenance,
    OutOfOrder
}
