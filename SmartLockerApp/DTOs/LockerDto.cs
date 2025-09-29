namespace SmartLockerApp.DTOs;

/// <summary>
/// DTO pour les casiers
/// Correspond au format JSON de l'API pour les lockers
/// </summary>
public class LockerDto
{
    public int id { get; set; }
    public string name { get; set; } = string.Empty;
    public string status { get; set; } = "available";
    public DateTime? created_at { get; set; }
    public DateTime? updated_at { get; set; }
    public DateTime? last_opened_at { get; set; }
}

/// <summary>
/// DTO pour créer un casier
/// </summary>
public class CreateLockerDto
{
    public string name { get; set; } = string.Empty;
    public string status { get; set; } = "available";
}

/// <summary>
/// DTO pour mettre à jour un casier
/// </summary>
public class UpdateLockerDto
{
    public string? name { get; set; }
    public string? status { get; set; }
}

/// <summary>
/// DTO pour la réponse d'ouverture de casier
/// </summary>
public class OpenLockerResponseDto
{
    public bool success { get; set; }
}
