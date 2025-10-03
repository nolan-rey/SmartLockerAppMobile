namespace SmartLockerApp.Models;

/// <summary>
/// Modèle représentant un utilisateur
/// Compatible avec l'API SmartLocker
/// </summary>
public class User
{
    public int id { get; set; }
    public string name { get; set; }
    public string email { get; set; }
    public string password_hash { get; set; }
    public string role { get; set; }
    public string created_at { get; set; }
}
