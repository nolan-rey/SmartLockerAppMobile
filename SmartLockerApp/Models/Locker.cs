namespace SmartLockerApp.Models;

/// <summary>
/// Modèle représentant un casier
/// </summary>
public class Locker
{
    public string Id { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public LockerSize Size { get; set; }
    public LockerStatus Status { get; set; }
    public decimal PricePerHour { get; set; }
    public List<string> Features { get; set; } = new();
    public string? CurrentSessionId { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
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
