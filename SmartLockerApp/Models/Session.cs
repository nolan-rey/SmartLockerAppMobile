using System.Text.Json.Serialization;

namespace SmartLockerApp.Models;

/// <summary>
/// Modèle représentant une session de casier
/// Compatible avec l'API SmartLocker
/// </summary>
public class Session
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("user_id")]
    public int UserId { get; set; }
    
    [JsonPropertyName("locker_id")]
    public int LockerId { get; set; }
    
    [JsonPropertyName("status")]
    public string Status { get; set; } = "active";
    
    [JsonPropertyName("started_at")]
    public string? StartedAt { get; set; }
    
    [JsonPropertyName("planned_end_at")]
    public string? PlannedEndAt { get; set; }
    
    [JsonPropertyName("ended_at")]
    public string? EndedAt { get; set; }
    
    [JsonPropertyName("amount_due")]
    public string AmountDue { get; set; } = "0.00";
    
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = "EUR";
    
    [JsonPropertyName("payment_status")]
    public string PaymentStatus { get; set; } = "none";
    
    // Propriétés calculées pour les dates
    public DateTime? StartedAtDate => string.IsNullOrEmpty(StartedAt) 
        ? null 
        : DateTime.TryParse(StartedAt, out var date) ? date : null;
    
    public DateTime? PlannedEndAtDate => string.IsNullOrEmpty(PlannedEndAt) 
        ? null 
        : DateTime.TryParse(PlannedEndAt, out var date) ? date : null;
    
    public DateTime? EndedAtDate => string.IsNullOrEmpty(EndedAt) || EndedAt == "null"
        ? null 
        : DateTime.TryParse(EndedAt, out var date) ? date : null;
    
    // Propriétés calculées pour les montants
    public decimal AmountDueDecimal => decimal.TryParse(AmountDue, out var amount) ? amount : 0m;
    
    // Propriétés calculées pour l'affichage
    public bool IsActive => Status?.ToLower() == "active";
    public bool IsFinished => Status?.ToLower() == "finished";
    public bool IsPaid => PaymentStatus?.ToLower() == "paid";
    
    // Temps restant
    public TimeSpan? RemainingTime
    {
        get
        {
            if (!IsActive || PlannedEndAtDate == null)
                return null;
                
            var remaining = PlannedEndAtDate.Value - DateTime.Now;
            return remaining.TotalSeconds > 0 ? remaining : TimeSpan.Zero;
        }
    }
    
    // Durée totale
    public TimeSpan? TotalDuration
    {
        get
        {
            if (StartedAtDate == null || PlannedEndAtDate == null)
                return null;
                
            return PlannedEndAtDate.Value - StartedAtDate.Value;
        }
    }
}
