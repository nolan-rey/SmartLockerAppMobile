namespace SmartLockerApp.Models;

/// <summary>
/// Modèle représentant un utilisateur
/// Compatible avec l'API SmartLocker
/// </summary>
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "user";
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Propriétés calculées pour compatibilité
    public string FirstName 
    { 
        get => Name.Split(' ').FirstOrDefault() ?? string.Empty;
        set => Name = $"{value} {LastName}".Trim();
    }
    
    public string LastName 
    { 
        get => string.Join(" ", Name.Split(' ').Skip(1));
        set => Name = $"{FirstName} {value}".Trim();
    }
    
    public string FullName => Name;
}
