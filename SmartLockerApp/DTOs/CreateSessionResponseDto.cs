namespace SmartLockerApp.DTOs;

/// <summary>
/// DTO pour la réponse de création de session
/// </summary>
public class CreateSessionResponseDto
{
    public bool success { get; set; }
    public string message { get; set; } = string.Empty;
    public SessionDto? session { get; set; }
}
