namespace SmartLockerApp.DTOs;

/// <summary>
/// DTO pour la réponse de connexion
/// Correspond au JSON retourné par POST /login
/// </summary>
public class LoginResponseDto
{
    public string token { get; set; } = string.Empty;
}
