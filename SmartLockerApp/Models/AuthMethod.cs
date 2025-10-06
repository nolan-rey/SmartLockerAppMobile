using System.Text.Json.Serialization;

namespace SmartLockerApp.Models;

/// <summary>
/// Modèle représentant une méthode d'authentification
/// Compatible avec l'API SmartLocker
/// </summary>
public class AuthMethod
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("user_id")]
    public int UserId { get; set; }
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = "rfid";
    
    [JsonPropertyName("credential_value")]
    public string CredentialValue { get; set; } = string.Empty;
    
    [JsonPropertyName("created_at")]
    public string? CreatedAt { get; set; }
    
    // Propriété calculée pour la date
    public DateTime? CreatedAtDate => string.IsNullOrEmpty(CreatedAt) 
        ? null 
        : DateTime.TryParse(CreatedAt, out var date) ? date : null;
    
    // Propriétés calculées pour le type
    public bool IsRfid => Type?.ToLower() == "rfid";
    public bool IsFingerprint => Type?.ToLower() == "fingerprint";
    
    // Affichage du type
    public string TypeDisplay => Type?.ToLower() switch
    {
        "rfid" => "Badge RFID",
        "fingerprint" => "Empreinte digitale",
        _ => Type ?? "Inconnu"
    };
}
