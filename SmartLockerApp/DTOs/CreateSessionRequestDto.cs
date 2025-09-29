namespace SmartLockerApp.DTOs;

/// <summary>
/// DTO pour créer une nouvelle session de casier
/// </summary>
public class CreateSessionRequestDto
{
    public int user_id { get; set; }
    public int locker_id { get; set; }
    public string start_time { get; set; } = string.Empty;
    public string end_time { get; set; } = string.Empty;
    public decimal cost { get; set; }
    public string status { get; set; } = "active";
}
