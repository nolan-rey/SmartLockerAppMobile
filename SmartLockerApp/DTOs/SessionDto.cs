namespace SmartLockerApp.DTOs;

/// <summary>
/// DTO pour les sessions
/// Correspond au format JSON de l'API pour les sessions
/// </summary>
public class SessionDto
{
    public int id { get; set; }
    public int user_id { get; set; }
    public int locker_id { get; set; }
    public string status { get; set; } = "active";
    public DateTime? started_at { get; set; }
    public DateTime? ended_at { get; set; }
    public DateTime? planned_end_at { get; set; }
    public decimal amount_due { get; set; }
    public string currency { get; set; } = "EUR";
    public string payment_status { get; set; } = "none";
    public DateTime? created_at { get; set; }
    public DateTime? updated_at { get; set; }
}

/// <summary>
/// DTO pour créer une session (CRUD low-level)
/// </summary>
public class CreateSessionDto
{
    public int user_id { get; set; }
    public int locker_id { get; set; }
    public string status { get; set; } = "active";
    public DateTime planned_end_at { get; set; }
    public decimal amount_due { get; set; } = 0;
    public string currency { get; set; } = "EUR";
    public string payment_status { get; set; } = "none";
}

/// <summary>
/// DTO pour démarrer une session (convenience)
/// </summary>
public class StartSessionDto
{
    public int user_id { get; set; }
    public int locker_id { get; set; }
    public DateTime planned_end_at { get; set; }
}

/// <summary>
/// DTO pour clôturer une session
/// </summary>
public class CloseSessionDto
{
    public string payment_status { get; set; } = "paid";
}

/// <summary>
/// DTO pour la réponse de démarrage de session
/// </summary>
public class StartSessionResponseDto
{
    public int id { get; set; }
    public string status { get; set; } = "active";
}
