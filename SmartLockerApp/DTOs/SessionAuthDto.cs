namespace SmartLockerApp.DTOs;

/// <summary>
/// DTO pour les liaisons Session/Authentification
/// Correspond au format JSON de l'API pour les session_auth
/// </summary>
public class SessionAuthDto
{
    public int id { get; set; }
    public int session_id { get; set; }
    public int auth_method_id { get; set; }
    public DateTime? created_at { get; set; }
}

/// <summary>
/// DTO pour créer une liaison Session/Authentification
/// </summary>
public class CreateSessionAuthDto
{
    public int session_id { get; set; }
    public int auth_method_id { get; set; }
}
