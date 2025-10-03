using SmartLockerApp.Models;

namespace SmartLockerApp.Services;

/// <summary>
/// Service de compatibilité pour la conversion entre différents formats d'IDs et statuts
/// Version simplifiée sans dépendance API
/// </summary>
public static class CompatibilityService
{
    #region Conversion d'IDs

    /// <summary>
    /// Convertit un ID string (ex: "A1", "L001") vers un ID int
    /// </summary>
    public static int StringToIntId(string? stringId)
    {
        if (string.IsNullOrWhiteSpace(stringId))
            return 0;

        // Format "A1", "B2", etc. -> extraire le chiffre
        if (stringId.Length == 2 && char.IsLetter(stringId[0]) && char.IsDigit(stringId[1]))
        {
            return int.Parse(stringId[1].ToString());
        }

        // Format "L001", "L002", etc. -> extraire les derniers chiffres
        if (stringId.StartsWith("L") && stringId.Length == 4)
        {
            return int.Parse(stringId.Substring(1));
        }

        // Si c'est déjà un nombre, le parser
        if (int.TryParse(stringId, out int result))
        {
            return result;
        }

        return 0;
    }

    /// <summary>
    /// Convertit un ID int vers un ID string (format "A1", "B2", "C3")
    /// </summary>
    public static string IntToStringId(int intId)
    {
        if (intId <= 0)
            return "A0";

        // Convertir 1->A1, 2->B2, 3->C3, etc.
        char letter = (char)('A' + (intId - 1) % 26);
        return $"{letter}{intId}";
    }

    /// <summary>
    /// Compare deux IDs (gère string et int)
    /// </summary>
    public static bool CompareIds(object? id1, object? id2)
    {
        if (id1 == null || id2 == null)
            return false;

        // Convertir les deux en int pour comparaison
        int int1 = id1 is string str1 ? StringToIntId(str1) : Convert.ToInt32(id1);
        int int2 = id2 is string str2 ? StringToIntId(str2) : Convert.ToInt32(id2);

        return int1 == int2;
    }

    #endregion

    #region Conversion de statuts

    /// <summary>
    /// Convertit un LockerStatus enum en string
    /// </summary>
    public static string StatusToString(LockerStatus status)
    {
        return status switch
        {
            LockerStatus.Available => "available",
            LockerStatus.Occupied => "occupied",
            LockerStatus.Maintenance => "maintenance",
            _ => "available"
        };
    }

    /// <summary>
    /// Convertit un string en LockerStatus enum
    /// </summary>
    public static LockerStatus StringToStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return LockerStatus.Available;

        return status.ToLower() switch
        {
            "available" => LockerStatus.Available,
            "occupied" => LockerStatus.Occupied,
            "maintenance" => LockerStatus.Maintenance,
            _ => LockerStatus.Available
        };
    }

    /// <summary>
    /// Compare un status string avec un LockerStatus enum
    /// </summary>
    public static bool CompareStatus(string? statusString, LockerStatus statusEnum)
    {
        if (string.IsNullOrWhiteSpace(statusString))
            return false;

        return StringToStatus(statusString) == statusEnum;
    }

    /// <summary>
    /// Convertit un SessionStatus enum en string
    /// </summary>
    public static string SessionStatusToString(SessionStatus status)
    {
        return status switch
        {
            SessionStatus.Active => "active",
            SessionStatus.Completed => "completed",
            SessionStatus.Cancelled => "cancelled",
            SessionStatus.Expired => "expired",
            _ => "active"
        };
    }

    /// <summary>
    /// Convertit un string en SessionStatus enum
    /// </summary>
    public static SessionStatus StringToSessionStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return SessionStatus.Active;

        return status.ToLower() switch
        {
            "active" => SessionStatus.Active,
            "completed" => SessionStatus.Completed,
            "cancelled" => SessionStatus.Cancelled,
            "expired" => SessionStatus.Expired,
            _ => SessionStatus.Active
        };
    }

    /// <summary>
    /// Compare un status string avec un SessionStatus enum
    /// </summary>
    public static bool CompareSessionStatus(string? statusString, SessionStatus statusEnum)
    {
        if (string.IsNullOrWhiteSpace(statusString))
            return false;

        return StringToSessionStatus(statusString) == statusEnum;
    }

    #endregion

    #region Utilitaires

    /// <summary>
    /// Mappe un ID d'affichage (A1, B2) vers un ID de service (L001, L002)
    /// </summary>
    public static string DisplayIdToServiceId(string displayId)
    {
        var numericId = StringToIntId(displayId);
        return $"L{numericId:D3}";
    }

    /// <summary>
    /// Mappe un ID de service (L001, L002) vers un ID d'affichage (A1, B2)
    /// </summary>
    public static string ServiceIdToDisplayId(string serviceId)
    {
        var numericId = StringToIntId(serviceId);
        return IntToStringId(numericId);
    }

    #endregion
}
