namespace SmartLockerApp.DTOs;

/// <summary>
/// DTO pour la création d'un nouvel utilisateur
/// </summary>
public class CreateUserRequestDto
{
    public string username { get; set; } = string.Empty;
    public string password { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public string role { get; set; } = "user"; // Par défaut "user"
}
