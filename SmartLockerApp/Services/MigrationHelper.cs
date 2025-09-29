using SmartLockerApp.Models;

namespace SmartLockerApp.Services;

/// <summary>
/// Helper pour faciliter la migration vers l'API
/// Corrige automatiquement les conversions de types
/// </summary>
public static class MigrationHelper
{
    /// <summary>
    /// Convertit un ID de session vers string de manière sécurisée
    /// </summary>
    public static string SessionIdToString(object sessionId)
    {
        return sessionId switch
        {
            int intId => CompatibilityService.IntToStringId(intId),
            string strId => strId,
            _ => sessionId?.ToString() ?? ""
        };
    }

    /// <summary>
    /// Convertit un ID de casier vers string de manière sécurisée
    /// </summary>
    public static string LockerIdToString(object lockerId)
    {
        return lockerId switch
        {
            int intId => CompatibilityService.IntToStringId(intId),
            string strId => strId,
            _ => lockerId?.ToString() ?? ""
        };
    }

    /// <summary>
    /// Compare un status de casier de manière sécurisée
    /// </summary>
    public static bool IsLockerAvailable(object status)
    {
        return status switch
        {
            string strStatus => CompatibilityService.CompareStatus(strStatus, LockerStatus.Available),
            LockerStatus enumStatus => enumStatus == LockerStatus.Available,
            _ => false
        };
    }

    /// <summary>
    /// Obtient la couleur d'un status de casier
    /// </summary>
    public static string GetStatusColor(object status)
    {
        return status switch
        {
            string strStatus when CompatibilityService.CompareStatus(strStatus, LockerStatus.Available) => "#10B981",
            string strStatus when CompatibilityService.CompareStatus(strStatus, LockerStatus.Occupied) => "#EF4444",
            string strStatus when CompatibilityService.CompareStatus(strStatus, LockerStatus.Maintenance) => "#F59E0B",
            LockerStatus.Available => "#10B981",
            LockerStatus.Occupied => "#EF4444",
            LockerStatus.Maintenance => "#F59E0B",
            _ => "#6B7280"
        };
    }

    /// <summary>
    /// Obtient le texte d'un status de casier
    /// </summary>
    public static string GetStatusText(object status)
    {
        return status switch
        {
            string strStatus when CompatibilityService.CompareStatus(strStatus, LockerStatus.Available) => "Disponible",
            string strStatus when CompatibilityService.CompareStatus(strStatus, LockerStatus.Occupied) => "Occupé",
            string strStatus when CompatibilityService.CompareStatus(strStatus, LockerStatus.Maintenance) => "Maintenance",
            LockerStatus.Available => "Disponible",
            LockerStatus.Occupied => "Occupé",
            LockerStatus.Maintenance => "Maintenance",
            _ => "Inconnu"
        };
    }

}
