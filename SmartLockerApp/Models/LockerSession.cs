namespace SmartLockerApp.Models;

/// <summary>
/// Modèle représentant une session de casier
/// Compatible avec l'API SmartLocker
/// </summary>
public class LockerSession
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int LockerId { get; set; }
    public string Status { get; set; } = "active";
    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public DateTime? PlannedEndAt { get; set; }
    public decimal AmountDue { get; set; }
    public string Currency { get; set; } = "EUR";
    public string PaymentStatus { get; set; } = "none";
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Propriétés calculées pour compatibilité avec l'ancien modèle
    public DateTime StartTime 
    { 
        get => StartedAt ?? DateTime.Now;
        set => StartedAt = value;
    }
    
    public DateTime EndTime 
    { 
        get => PlannedEndAt ?? DateTime.Now.AddHours(1);
        set => PlannedEndAt = value;
    }
    
    public DateTime? ActualEndTime 
    { 
        get => EndedAt;
        set => EndedAt = value;
    }
    
    public int DurationHours 
    { 
        get => PlannedEndAt.HasValue && StartedAt.HasValue 
            ? (int)(PlannedEndAt.Value - StartedAt.Value).TotalHours 
            : 1;
        set => PlannedEndAt = StartedAt?.AddHours(value) ?? DateTime.Now.AddHours(value);
    }
    
    public decimal TotalCost 
    { 
        get => AmountDue;
        set => AmountDue = value;
    }
    
    public SessionStatus StatusEnum 
    { 
        get => Status.ToLower() switch
        {
            "active" => SessionStatus.Active,
            "finished" => SessionStatus.Completed,
            "expired" => SessionStatus.Expired,
            "cancelled" => SessionStatus.Cancelled,
            _ => SessionStatus.Active
        };
        set => Status = value switch
        {
            SessionStatus.Active => "active",
            SessionStatus.Completed => "finished",
            SessionStatus.Expired => "expired",
            SessionStatus.Cancelled => "cancelled",
            _ => "active"
        };
    }
    
    public List<string> Items { get; set; } = new();
    public bool IsLocked { get; set; } = true;
    public DateTime? LockedAt { get; set; }
    public string? Notes { get; set; }
    
    // Propriétés calculées pour l'affichage
    public string Location { get; set; } = string.Empty;
    public string RemainingTime 
    { 
        get 
        {
            if (StatusEnum != SessionStatus.Active) return "Session terminée";
            
            var remaining = EndTime - DateTime.Now;
            if (remaining.TotalMinutes <= 0) return "Expiré";
            
            if (remaining.TotalHours >= 1)
                return $"{remaining.Hours}h {remaining.Minutes}min";
            else
                return $"{remaining.Minutes}min";
        } 
    }
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
