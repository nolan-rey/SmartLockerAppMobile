namespace SmartLockerApp.DTOs;

/// <summary>
/// DTO pour les utilisateurs
/// Correspond au format JSON de l'API pour les users
/// </summary>
public class UserDto
{
    public int id { get; set; }
    public string name { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string? password_hash { get; set; }
    public string role { get; set; } = "user";
    public DateTime? created_at { get; set; }
    public DateTime? updated_at { get; set; }
}

/// <summary>
/// DTO pour créer un utilisateur
/// </summary>
public class CreateUserDto
{
    public string name { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string password_hash { get; set; } = string.Empty;
    public string role { get; set; } = "user";
}

/// <summary>
/// DTO pour mettre à jour un utilisateur
/// </summary>
public class UpdateUserDto
{
    public string? name { get; set; }
    public string? email { get; set; }
    public string? role { get; set; }
}
