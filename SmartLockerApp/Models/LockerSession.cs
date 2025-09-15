namespace SmartLockerApp.Models;

/// <summary>
/// Modèle représentant une session de casier
/// </summary>
public class LockerSession
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string LockerId { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime? ActualEndTime { get; set; }
    public int DurationHours { get; set; }
    public decimal TotalCost { get; set; }
    public SessionStatus Status { get; set; }
    public List<string> Items { get; set; } = new();
    public bool IsLocked { get; set; }
    public DateTime? LockedAt { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Statut de la session
/// </summary>
public enum SessionStatus
{
    Active,
    Completed,
    Expired,
    Cancelled
}
