using SmartLockerApp.Models;
using SmartLockerApp.Services;

namespace SmartLockerApp.Extensions;

/// <summary>
/// Extensions pour faciliter la migration vers l'API
/// </summary>
public static class MigrationExtensions
{
    public static async Task<LockerSession?> GetSessionAsync(this AppStateService appState, object sessionId)
    {
        var strId = MigrationHelper.SessionIdToString(sessionId);
        return await appState.GetSessionAsync(strId);
    }

    public static Locker? GetLockerDetails(this AppStateService appState, object lockerId)
    {
        var strId = MigrationHelper.LockerIdToString(lockerId);
        return appState.GetLockerDetails(strId);
    }

    public static async Task<bool> EndSessionAsync(this AppStateService appState, object sessionId)
    {
        var strId = MigrationHelper.SessionIdToString(sessionId);
        var result = await appState.EndSessionAsync(strId);
        return result.Success;
    }
}
