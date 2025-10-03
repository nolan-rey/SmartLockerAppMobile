using SmartLockerApp.Services;
using SmartLockerApp.Models;

namespace SmartLockerApp.Extensions;

/// <summary>
/// Extensions pour simplifier l'utilisation des services
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Extension pour vérifier si un utilisateur est connecté
    /// </summary>
    public static bool IsUserLoggedIn(this AppStateService appState) => 
        appState.IsLoggedIn;

    /// <summary>
    /// Extension pour obtenir le nom complet de l'utilisateur connecté
    /// </summary>
    public static string GetCurrentUserFullName(this AppStateService appState) =>
        appState.CurrentUser?.name ?? "Utilisateur";

    /// <summary>
    /// Extension pour obtenir le nombre de sessions actives
    /// </summary>
    public static int GetActiveSessionsCount(this AppStateService appState) =>
        appState.ActiveSession != null ? 1 : 0;

    /// <summary>
    /// Extension pour obtenir les casiers disponibles seulement
    /// </summary>
    public static List<Locker> GetAvailableLockers(this AppStateService appState) =>
        appState.Lockers.Where(l => CompatibilityService.CompareStatus(l.Status, LockerStatus.Available)).ToList();

    /// <summary>
    /// Extension pour calculer le temps restant d'une session
    /// </summary>
    public static TimeSpan GetSessionRemainingTime(this LockerSession session)
    {
        var endTime = session.StartTime.AddHours(session.DurationHours);
        var remaining = endTime - DateTime.Now;
        return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
    }

    /// <summary>
    /// Extension pour formater le temps restant en texte lisible
    /// </summary>
    public static string FormatRemainingTime(this LockerSession session)
    {
        var remaining = session.GetSessionRemainingTime();
        if (remaining == TimeSpan.Zero)
            return "Expiré";

        if (remaining.TotalHours >= 1)
            return $"{remaining.Hours}h {remaining.Minutes}min";
        
        return $"{remaining.Minutes}min";
    }

    /// <summary>
    /// Extension pour vérifier si une session est expirée
    /// </summary>
    public static bool IsExpired(this LockerSession session) =>
        session.GetSessionRemainingTime() == TimeSpan.Zero;

    /// <summary>
    /// Extension pour obtenir la couleur de statut d'un casier
    /// </summary>
    public static Color GetStatusColor(this Locker locker)
    {
        if (CompatibilityService.CompareStatus(locker.Status, LockerStatus.Available))
            return Colors.Green;
        if (CompatibilityService.CompareStatus(locker.Status, LockerStatus.Occupied))
            return Colors.Red;
        if (CompatibilityService.CompareStatus(locker.Status, LockerStatus.Maintenance))
            return Colors.Orange;
        return Colors.Gray;
    }

    /// <summary>
    /// Extension pour obtenir le texte de statut d'un casier
    /// </summary>
    public static string GetStatusText(this Locker locker)
    {
        if (CompatibilityService.CompareStatus(locker.Status, LockerStatus.Available))
            return "Disponible";
        if (CompatibilityService.CompareStatus(locker.Status, LockerStatus.Occupied))
            return "Occupé";
        if (CompatibilityService.CompareStatus(locker.Status, LockerStatus.Maintenance))
            return "Maintenance";
        return "Inconnu";
    }

    /// <summary>
    /// Extension pour exécuter une animation avec gestion d'erreur
    /// </summary>
    public static async Task SafeAnimateAsync(this VisualElement element, Func<VisualElement, Task> animation)
    {
        try
        {
            if (element != null)
                await animation(element);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Animation error: {ex.Message}");
        }
    }

    /// <summary>
    /// Extension pour animer plusieurs éléments en parallèle
    /// </summary>
    public static async Task AnimateAllAsync(this IEnumerable<VisualElement> elements, Func<VisualElement, Task> animation)
    {
        var tasks = elements.Select(element => element.SafeAnimateAsync(animation));
        await Task.WhenAll(tasks);
    }
}
