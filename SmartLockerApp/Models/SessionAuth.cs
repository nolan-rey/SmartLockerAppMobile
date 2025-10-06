using System.Text.Json.Serialization;

namespace SmartLockerApp.Models;

/// <summary>
/// Modèle représentant une liaison Session/Authentification
/// Compatible avec l'API SmartLocker
/// </summary>
public class SessionAuth
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("session_id")]
    public int SessionId { get; set; }
    
    [JsonPropertyName("auth_method_id")]
    public int AuthMethodId { get; set; }
    
    [JsonPropertyName("created_at")]
    public string? CreatedAt { get; set; }
    
    // Propriété calculée pour la date
    public DateTime? CreatedAtDate => string.IsNullOrEmpty(CreatedAt) 
        ? null 
        : DateTime.TryParse(CreatedAt, out var date) ? date : null;
}
