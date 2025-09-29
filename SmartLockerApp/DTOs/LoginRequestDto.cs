namespace SmartLockerApp.DTOs;

/// <summary>
/// DTO pour la requête de connexion
/// Correspond exactement au JSON attendu par POST /login
/// </summary>
public class LoginRequestDto
{
    public string username { get; set; } = string.Empty;
    public string password { get; set; } = string.Empty;
}
