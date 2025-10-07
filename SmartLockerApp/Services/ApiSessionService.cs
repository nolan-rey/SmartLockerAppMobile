using SmartLockerApp.Models;

namespace SmartLockerApp.Services;

/// <summary>
/// Service de gestion des sessions via l'API
/// Utilise ApiHttpClient avec authentification JWT automatique
/// </summary>
public class ApiSessionService
{
    private readonly ApiHttpClient _apiClient;

    public ApiSessionService(ApiHttpClient apiClient)
    {
        _apiClient = apiClient;
    }

    #region GET - Récupération

    /// <summary>
    /// GET /sessions - Récupère toutes les sessions
    /// </summary>
    public async Task<List<Session>?> GetAllSessionsAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("📋 Récupération de toutes les sessions...");
            
            var sessions = await _apiClient.GetAsync<List<Session>>("sessions");
            
            if (sessions != null)
            {
                System.Diagnostics.Debug.WriteLine($"✅ {sessions.Count} sessions récupérées");
                foreach (var session in sessions.Take(5))
                {
                    System.Diagnostics.Debug.WriteLine($"   - Session #{session.Id}: User {session.UserId}, Locker {session.LockerId} ({session.Status})");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("⚠️ Aucune session récupérée");
            }
            
            return sessions;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur GetAllSessions: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// GET /sessions/{id} - Récupère une session par ID
    /// </summary>
    public async Task<Session?> GetSessionByIdAsync(int sessionId)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"🔍 Récupération session ID={sessionId}...");
            
            var session = await _apiClient.GetAsync<Session>($"sessions/{sessionId}");
            
            if (session != null)
            {
                System.Diagnostics.Debug.WriteLine($"✅ Session trouvée: User {session.UserId}, Locker {session.LockerId} ({session.Status})");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("⚠️ Session non trouvée");
            }
            
            return session;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur GetSessionById: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// GET /me/sessions?status=active - Récupère les sessions actives de l'utilisateur connecté
    /// </summary>
    public async Task<List<Session>?> GetMyActiveSessionsAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("📋 Récupération des sessions actives de l'utilisateur...");
            
            var sessions = await _apiClient.GetAsync<List<Session>>("me/sessions?status=active");
            
            if (sessions != null)
            {
                System.Diagnostics.Debug.WriteLine($"✅ {sessions.Count} session(s) active(s)");
                foreach (var session in sessions)
                {
                    System.Diagnostics.Debug.WriteLine($"   - Session #{session.Id}: Locker {session.LockerId}");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("⚠️ Aucune session active");
            }
            
            return sessions;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur GetMyActiveSessions: {ex.Message}");
            return null;
        }
    }

    #endregion

    #region POST - Création

    /// <summary>
    /// POST /sessions - Crée une nouvelle session
    /// Body: {"user_id": 1, "locker_id": 1, "status": "active"}
    /// </summary>
    public async Task<(bool Success, string Message, Session? Session)> CreateSessionAsync(
        int userId,
        int lockerId,
        string status = "active",
        string? plannedEndAt = null,
        decimal? amountDue = null,
        string currency = "EUR",
        string paymentStatus = "none")
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"➕ Création session: User {userId}, Locker {lockerId}...");

            var sessionData = new
            {
                user_id = userId,
                locker_id = lockerId,
                status = status,
                planned_end_at = plannedEndAt,
                amount_due = amountDue,
                currency = currency,
                payment_status = paymentStatus
            };

            var response = await _apiClient.PostAsync<object, SuccessResponse>("sessions", sessionData);

            if (response?.success == true)
            {
                System.Diagnostics.Debug.WriteLine($"✅ Session créée");
                
                // Récupérer la session créée (l'API retourne juste success: true)
                var sessions = await GetMyActiveSessionsAsync();
                var createdSession = sessions?.FirstOrDefault(s => s.UserId == userId && s.LockerId == lockerId);
                
                return (true, "Session créée avec succès", createdSession);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("❌ Échec création session");
                return (false, "Échec de la création de la session", null);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur CreateSession: {ex.Message}");
            return (false, $"Erreur: {ex.Message}", null);
        }
    }

    /// <summary>
    /// Crée une session avec durée en heures
    /// </summary>
    public async Task<(bool Success, string Message, Session? Session)> CreateSessionWithDurationAsync(
        int userId,
        int lockerId,
        int durationHours,
        decimal pricePerHour = 2.50m)
    {
        var plannedEndAt = DateTime.Now.AddHours(durationHours).ToString("yyyy-MM-dd HH:mm:ss");
        var amountDue = durationHours * pricePerHour;

        return await CreateSessionAsync(
            userId,
            lockerId,
            "active",
            plannedEndAt,
            amountDue,
            "EUR",
            "none"
        );
    }

    #endregion

    #region PUT - Mise à jour

    /// <summary>
    /// PUT /sessions/{id} - Met à jour une session
    /// Body: {"status": "finished", "ended_at": "2025-12-31 19:00:00", "payment_status": "paid"}
    /// </summary>
    public async Task<(bool Success, string Message)> UpdateSessionAsync(
        int sessionId,
        string? status = null,
        string? endedAt = null,
        string? paymentStatus = null)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"✏️ Mise à jour session ID={sessionId}...");

            var updateData = new
            {
                status = status,
                ended_at = endedAt,
                payment_status = paymentStatus
            };

            var response = await _apiClient.PutAsync<object, SuccessResponse>($"sessions/{sessionId}", updateData);

            if (response?.success == true)
            {
                System.Diagnostics.Debug.WriteLine($"✅ Session mise à jour");
                return (true, "Session mise à jour avec succès");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("❌ Échec mise à jour session");
                return (false, "Échec de la mise à jour de la session");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur UpdateSession: {ex.Message}");
            return (false, $"Erreur: {ex.Message}");
        }
    }

    /// <summary>
    /// Termine une session (status=finished, ended_at=now)
    /// </summary>
    public async Task<(bool Success, string Message)> FinishSessionAsync(
        int sessionId,
        string paymentStatus = "paid")
    {
        var endedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        return await UpdateSessionAsync(sessionId, "finished", endedAt, paymentStatus);
    }

    #endregion

    #region DELETE - Suppression

    /// <summary>
    /// DELETE /sessions/{id} - Supprime une session
    /// </summary>
    public async Task<(bool Success, string Message)> DeleteSessionAsync(int sessionId)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"🗑️ Suppression session ID={sessionId}...");

            var success = await _apiClient.DeleteAsync($"sessions/{sessionId}");

            if (success)
            {
                System.Diagnostics.Debug.WriteLine("✅ Session supprimée");
                return (true, "Session supprimée avec succès");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("❌ Échec suppression session");
                return (false, "Échec de la suppression de la session");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur DeleteSession: {ex.Message}");
            return (false, $"Erreur: {ex.Message}");
        }
    }

    #endregion

    #region Actions spéciales

    /// <summary>
    /// POST /sessions/{id}/close - Clôture une session
    /// Body: {"payment_status": "paid"}
    /// Effets: status='finished', ended_at=NOW(), locker.status='available'
    /// </summary>
    public async Task<(bool Success, string Message)> CloseSessionAsync(
        int sessionId,
        string paymentStatus = "paid")
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"🔒 Clôture session ID={sessionId}...");

            var closeData = new
            {
                payment_status = paymentStatus
            };

            var success = await _apiClient.PostAsync($"sessions/{sessionId}/close", closeData);

            if (success)
            {
                System.Diagnostics.Debug.WriteLine("✅ Session clôturée");
                return (true, "Session clôturée avec succès");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("❌ Échec clôture session");
                return (false, "Échec de la clôture de la session");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur CloseSession: {ex.Message}");
            return (false, $"Erreur: {ex.Message}");
        }
    }

    #endregion

    #region Helpers & Statistiques

    /// <summary>
    /// Récupère les sessions par statut
    /// </summary>
    public async Task<List<Session>?> GetSessionsByStatusAsync(string status)
    {
        try
        {
            var allSessions = await GetAllSessionsAsync();
            
            if (allSessions == null)
                return null;

            return allSessions
                .Where(s => s.Status.Equals(status, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur GetSessionsByStatus: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Récupère les sessions actives
    /// </summary>
    public async Task<List<Session>?> GetActiveSessionsAsync()
    {
        return await GetSessionsByStatusAsync("active");
    }

    /// <summary>
    /// Récupère les sessions terminées
    /// </summary>
    public async Task<List<Session>?> GetFinishedSessionsAsync()
    {
        return await GetSessionsByStatusAsync("finished");
    }

    /// <summary>
    /// Récupère les sessions d'un utilisateur
    /// </summary>
    public async Task<List<Session>?> GetUserSessionsAsync(int userId)
    {
        try
        {
            var allSessions = await GetAllSessionsAsync();
            
            if (allSessions == null)
                return null;

            return allSessions.Where(s => s.UserId == userId).ToList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur GetUserSessions: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Récupère les sessions d'un casier
    /// </summary>
    public async Task<List<Session>?> GetLockerSessionsAsync(int lockerId)
    {
        try
        {
            var allSessions = await GetAllSessionsAsync();
            
            if (allSessions == null)
                return null;

            return allSessions.Where(s => s.LockerId == lockerId).ToList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur GetLockerSessions: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Calcule les statistiques des sessions
    /// </summary>
    public async Task<SessionStats?> GetSessionStatisticsAsync()
    {
        try
        {
            var sessions = await GetAllSessionsAsync();
            
            if (sessions == null || sessions.Count == 0)
                return null;

            var stats = new SessionStats
            {
                TotalSessions = sessions.Count,
                ActiveSessions = sessions.Count(s => s.IsActive),
                FinishedSessions = sessions.Count(s => s.IsFinished),
                PaidSessions = sessions.Count(s => s.IsPaid),
                TotalRevenue = sessions.Sum(s => s.AmountDueDecimal),
                AverageSessionDuration = sessions
                    .Where(s => s.TotalDuration.HasValue)
                    .Average(s => s.TotalDuration!.Value.TotalHours)
            };

            System.Diagnostics.Debug.WriteLine("📊 Statistiques sessions:");
            System.Diagnostics.Debug.WriteLine($"   - Total: {stats.TotalSessions}");
            System.Diagnostics.Debug.WriteLine($"   - Actives: {stats.ActiveSessions}");
            System.Diagnostics.Debug.WriteLine($"   - Terminées: {stats.FinishedSessions}");
            System.Diagnostics.Debug.WriteLine($"   - Payées: {stats.PaidSessions}");
            System.Diagnostics.Debug.WriteLine($"   - Revenu total: {stats.TotalRevenue:F2} EUR");

            return stats;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur GetSessionStatistics: {ex.Message}");
            return null;
        }
    }

    #endregion

    #region Tests

    /// <summary>
    /// Teste toutes les opérations CRUD
    /// </summary>
    public async Task TestAllOperationsAsync()
    {
        System.Diagnostics.Debug.WriteLine("\n=== TEST DES OPÉRATIONS CRUD SESSIONS ===\n");

        // 1. GET ALL
        System.Diagnostics.Debug.WriteLine("1️⃣ Test GET /sessions");
        var allSessions = await GetAllSessionsAsync();
        if (allSessions != null)
        {
            System.Diagnostics.Debug.WriteLine($"   ✅ {allSessions.Count} sessions récupérées");
        }

        await Task.Delay(500);

        // 2. GET MY ACTIVE
        System.Diagnostics.Debug.WriteLine("\n2️⃣ Test GET /me/sessions?status=active");
        var myActiveSessions = await GetMyActiveSessionsAsync();
        if (myActiveSessions != null)
        {
            System.Diagnostics.Debug.WriteLine($"   ✅ {myActiveSessions.Count} session(s) active(s)");
        }

        await Task.Delay(500);

        // 3. GET BY ID
        System.Diagnostics.Debug.WriteLine("\n3️⃣ Test GET /sessions/1");
        var session = await GetSessionByIdAsync(1);
        if (session != null)
        {
            System.Diagnostics.Debug.WriteLine($"   ✅ Session: User {session.UserId}, Locker {session.LockerId}");
        }

        await Task.Delay(500);

        // 4. CREATE
        System.Diagnostics.Debug.WriteLine("\n4️⃣ Test POST /sessions");
        var (createSuccess, createMsg, newSession) = await CreateSessionWithDurationAsync(1, 1, 2);
        System.Diagnostics.Debug.WriteLine($"   {(createSuccess ? "✅" : "❌")} {createMsg}");

        await Task.Delay(500);

        if (newSession != null)
        {
            // 5. UPDATE
            System.Diagnostics.Debug.WriteLine("\n5️⃣ Test PUT /sessions/{id}");
            var (updateSuccess, updateMsg) = await UpdateSessionAsync(newSession.Id, paymentStatus: "paid");
            System.Diagnostics.Debug.WriteLine($"   {(updateSuccess ? "✅" : "❌")} {updateMsg}");

            await Task.Delay(500);

            // 6. CLOSE
            System.Diagnostics.Debug.WriteLine("\n6️⃣ Test POST /sessions/{id}/close");
            var (closeSuccess, closeMsg) = await CloseSessionAsync(newSession.Id, "paid");
            System.Diagnostics.Debug.WriteLine($"   {(closeSuccess ? "✅" : "❌")} {closeMsg}");

            await Task.Delay(500);

            // 7. DELETE
            System.Diagnostics.Debug.WriteLine("\n7️⃣ Test DELETE /sessions/{id}");
            var (deleteSuccess, deleteMsg) = await DeleteSessionAsync(newSession.Id);
            System.Diagnostics.Debug.WriteLine($"   {(deleteSuccess ? "✅" : "❌")} {deleteMsg}");
        }

        await Task.Delay(500);

        // 8. STATISTICS
        System.Diagnostics.Debug.WriteLine("\n8️⃣ Test Statistiques");
        var stats = await GetSessionStatisticsAsync();
        if (stats != null)
        {
            System.Diagnostics.Debug.WriteLine($"   ✅ Statistiques récupérées");
        }

        System.Diagnostics.Debug.WriteLine("\n=== FIN DES TESTS ===\n");
    }

    #endregion

    #region DTOs

    /// <summary>
    /// DTO pour les réponses de succès simples
    /// </summary>
    private class SuccessResponse
    {
        public bool success { get; set; }
    }

    /// <summary>
    /// Statistiques des sessions
    /// </summary>
    public class SessionStats
    {
        public int TotalSessions { get; set; }
        public int ActiveSessions { get; set; }
        public int FinishedSessions { get; set; }
        public int PaidSessions { get; set; }
        public decimal TotalRevenue { get; set; }
        public double AverageSessionDuration { get; set; }
    }

    #endregion
}
