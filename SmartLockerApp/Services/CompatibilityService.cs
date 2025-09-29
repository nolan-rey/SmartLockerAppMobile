using SmartLockerApp.Models;

namespace SmartLockerApp.Services;

/// <summary>
/// Service de compatibilité pour faciliter la migration vers l'API
/// Convertit entre les anciens formats (string IDs) et les nouveaux (int IDs)
/// </summary>
public static class CompatibilityService
{
    #region ID Mapping

    /// <summary>
    /// Convertit un ID string vers int (pour compatibilité avec l'API)
    /// </summary>
    public static int StringToIntId(string stringId)
    {
        // Mapping spécifique pour les anciens IDs
        return stringId switch
        {
            "A1" => 1,
            "B2" => 2,
            "C3" => 3,
            "L001" => 1,
            "L002" => 2,
            "L003" => 3,
            _ => int.TryParse(stringId, out int id) ? id : 0
        };
    }

    /// <summary>
    /// Convertit un ID int vers string (pour compatibilité avec l'ancien code)
    /// </summary>
    public static string IntToStringId(int intId)
    {
        return intId switch
        {
            1 => "A1",
            2 => "B2", 
            3 => "C3",
            _ => intId.ToString()
        };
    }

    #endregion

    #region Status Conversion

    /// <summary>
    /// Convertit un LockerStatus enum vers string
    /// </summary>
    public static string StatusToString(LockerStatus status)
    {
        return status switch
        {
            LockerStatus.Available => "available",
            LockerStatus.Occupied => "occupied",
            LockerStatus.Maintenance => "maintenance",
            LockerStatus.OutOfOrder => "out_of_order",
            _ => "available"
        };
    }

    /// <summary>
    /// Convertit un string vers LockerStatus enum
    /// </summary>
    public static LockerStatus StringToStatus(string status)
    {
        return status.ToLower() switch
        {
            "available" => LockerStatus.Available,
            "occupied" => LockerStatus.Occupied,
            "maintenance" => LockerStatus.Maintenance,
            "out_of_order" => LockerStatus.OutOfOrder,
            _ => LockerStatus.Available
        };
    }

    /// <summary>
    /// Convertit un SessionStatus enum vers string
    /// </summary>
    public static string SessionStatusToString(SessionStatus status)
    {
        return status switch
        {
            SessionStatus.Active => "active",
            SessionStatus.Completed => "finished",
            SessionStatus.Expired => "expired",
            SessionStatus.Cancelled => "cancelled",
            _ => "active"
        };
    }

    /// <summary>
    /// Convertit un string vers SessionStatus enum
    /// </summary>
    public static SessionStatus StringToSessionStatus(string status)
    {
        return status.ToLower() switch
        {
            "active" => SessionStatus.Active,
            "finished" => SessionStatus.Completed,
            "expired" => SessionStatus.Expired,
            "cancelled" => SessionStatus.Cancelled,
            _ => SessionStatus.Active
        };
    }

    #endregion

    #region Model Helpers

    /// <summary>
    /// Crée un Locker compatible avec l'ancien code
    /// </summary>
    public static Locker CreateCompatibleLocker(int id, string name, string status)
    {
        return new Locker
        {
            Id = id,
            Name = name,
            Status = status,
            Size = LockerSize.Medium,
            PricePerHour = 2.0m,
            Features = new List<string> { "Sécurisé", "Climatisé" }
        };
    }

    /// <summary>
    /// Crée une LockerSession compatible avec l'ancien code
    /// </summary>
    public static LockerSession CreateCompatibleSession(int id, int userId, int lockerId, string status)
    {
        return new LockerSession
        {
            Id = id,
            UserId = userId,
            LockerId = lockerId,
            Status = status,
            StartedAt = DateTime.Now,
            PlannedEndAt = DateTime.Now.AddHours(2),
            AmountDue = 4.0m,
            Currency = "EUR",
            PaymentStatus = "none",
            IsLocked = true,
            Items = new List<string>()
        };
    }

    #endregion

    #region Safe Comparisons

    /// <summary>
    /// Compare un ID int avec un ID string de manière sécurisée
    /// </summary>
    public static bool CompareIds(int intId, string stringId)
    {
        return intId == StringToIntId(stringId);
    }

    /// <summary>
    /// Compare un status string avec un enum de manière sécurisée
    /// </summary>
    public static bool CompareStatus(string stringStatus, LockerStatus enumStatus)
    {
        return stringStatus == StatusToString(enumStatus);
    }

    /// <summary>
    /// Compare un status string avec un enum SessionStatus de manière sécurisée
    /// </summary>
    public static bool CompareSessionStatus(string stringStatus, SessionStatus enumStatus)
    {
        return stringStatus == SessionStatusToString(enumStatus);
    }

    #endregion
}
