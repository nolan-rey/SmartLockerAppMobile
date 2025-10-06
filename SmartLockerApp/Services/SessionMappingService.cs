using SmartLockerApp.Models;

namespace SmartLockerApp.Services;

/// <summary>
/// Service de mapping entre Session (API) et LockerSession (App)
/// </summary>
public static class SessionMappingService
{
    /// <summary>
    /// Convertit un Session (API) en LockerSession (App)
    /// </summary>
    public static LockerSession ToLockerSession(this Session session)
    {
        return new LockerSession
        {
            Id = session.Id,
            UserId = session.UserId,
            LockerId = session.LockerId,
            Status = session.Status,
            StartedAt = session.StartedAtDate,
            EndedAt = session.EndedAtDate,
            PlannedEndAt = session.PlannedEndAtDate,
            AmountDue = session.AmountDueDecimal,
            Currency = session.Currency,
            PaymentStatus = session.PaymentStatus,
            CreatedAt = session.StartedAtDate,
            UpdatedAt = DateTime.Now
        };
    }

    /// <summary>
    /// Convertit un LockerSession (App) en Session (API)
    /// </summary>
    public static Session ToSession(this LockerSession lockerSession)
    {
        return new Session
        {
            Id = lockerSession.Id,
            UserId = lockerSession.UserId,
            LockerId = lockerSession.LockerId,
            Status = lockerSession.Status,
            StartedAt = lockerSession.StartedAt?.ToString("yyyy-MM-dd HH:mm:ss"),
            EndedAt = lockerSession.EndedAt?.ToString("yyyy-MM-dd HH:mm:ss"),
            PlannedEndAt = lockerSession.PlannedEndAt?.ToString("yyyy-MM-dd HH:mm:ss"),
            AmountDue = lockerSession.AmountDue.ToString("F2"),
            Currency = lockerSession.Currency,
            PaymentStatus = lockerSession.PaymentStatus
        };
    }

    /// <summary>
    /// Convertit une liste de Session en liste de LockerSession
    /// </summary>
    public static List<LockerSession> ToLockerSessions(this List<Session>? sessions)
    {
        if (sessions == null) return new List<LockerSession>();
        return sessions.Select(s => s.ToLockerSession()).ToList();
    }
}
