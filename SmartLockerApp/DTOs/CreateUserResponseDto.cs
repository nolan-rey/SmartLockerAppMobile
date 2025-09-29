namespace SmartLockerApp.DTOs;

/// <summary>
/// DTO pour la réponse de création d'utilisateur
/// </summary>
public class CreateUserResponseDto
{
    public bool success { get; set; }
    public string message { get; set; } = string.Empty;
    public UserDto? user { get; set; }
}
