namespace SmartLockerApp.DTOs;

/// <summary>
/// DTO pour les méthodes d'authentification
/// Correspond au format JSON de l'API pour les auth_methods
/// </summary>
public class AuthMethodDto
{
    public int id { get; set; }
    public int user_id { get; set; }
    public string type { get; set; } = string.Empty; // "rfid", "fingerprint", etc.
    public string credential_value { get; set; } = string.Empty;
    public DateTime? created_at { get; set; }
    public DateTime? updated_at { get; set; }
}

/// <summary>
/// DTO pour créer une méthode d'authentification
/// </summary>
public class CreateAuthMethodDto
{
    public int user_id { get; set; }
    public string type { get; set; } = string.Empty;
    public string credential_value { get; set; } = string.Empty;
}

/// <summary>
/// DTO pour mettre à jour une méthode d'authentification
/// </summary>
public class UpdateAuthMethodDto
{
    public string? type { get; set; }
    public string? credential_value { get; set; }
}
