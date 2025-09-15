namespace SmartLockerApp.Models;

/// <summary>
/// Modèle représentant un utilisateur
/// </summary>
public class User
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
}
