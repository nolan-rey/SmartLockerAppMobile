using SmartLockerApp.Services;
using SmartLockerApp.Extensions;
using SmartLockerApp.Models;

namespace SmartLockerApp.Examples;

/// <summary>
/// Exemples d'utilisation des services optimisés SmartLocker
/// Ces exemples montrent comment utiliser les nouveaux services simplifiés
/// </summary>
public static class ServiceUsageExamples
{
    /// <summary>
    /// Exemple 1: Validation et notifications simplifiées
    /// </summary>
    public static async Task LoginExample(string email, string password)
    {
        // Validation avec les nouvelles extensions
        var (isValid, errors) = ValidationService.ValidateLogin(email, password);
        
        if (!isValid)
        {
            // Affichage des erreurs avec les extensions string
            var errorMessage = string.Join(", ", errors);
            await errorMessage.ShowErrorAsync();
            return;
        }

        // Authentification avec le service optimisé
        var appState = AppStateService.Instance;
        var success = await appState.LoginAsync(email, password);
        
        if (success)
        {
            // Notification de succès avec méthodes rapides
            await NotificationService.Quick.LoginSuccessAsync();
        }
        else
        {
            await NotificationService.Quick.LoginErrorAsync();
        }
    }

    /// <summary>
    /// Exemple 2: Animations sécurisées avec extensions
    /// </summary>
    public static async Task AnimationExample(List<VisualElement> elements)
    {
        // Animation de tous les éléments en parallèle avec gestion d'erreur automatique
        await elements.AnimateAllAsync(async element => 
            await AnimationService.SlideInFromBottomAsync(element));

        // Animation d'attention sur un élément spécifique
        var importantElement = elements.FirstOrDefault();
        if (importantElement != null)
        {
            await importantElement.SafeAnimateAsync(AnimationService.AttentionAsync);
        }
    }

    /// <summary>
    /// Exemple 3: Gestion des sessions avec extensions
    /// </summary>
    public static async Task SessionExample()
    {
        var appState = AppStateService.Instance;
        var activeSession = appState.ActiveSession;
        
        if (activeSession != null)
        {
            // Utilisation des extensions pour les sessions
            var remainingTime = activeSession.FormatRemainingTime();
            var isExpired = activeSession.IsExpired();
            
            if (isExpired)
            {
                await "Votre session a expiré".ShowErrorAsync();
            }
            else
            {
                await $"Temps restant: {remainingTime}".ShowInfoAsync();
            }
        }
    }

    /// <summary>
    /// Exemple 4: Gestion des casiers avec extensions
    /// </summary>
    public static async Task LockerExample()
    {
        var appState = AppStateService.Instance;
        var availableLockers = appState.GetAvailableLockers();
        
        foreach (var locker in availableLockers)
        {
            // Utilisation des extensions pour les casiers
            var statusColor = locker.GetStatusColor();
            var statusText = locker.GetStatusText();
            
            System.Diagnostics.Debug.WriteLine($"Casier {locker.Id}: {statusText}");
        }
    }

    /// <summary>
    /// Exemple 5: Stockage local optimisé
    /// </summary>
    public static async Task StorageExample()
    {
        var storage = LocalStorageService.Instance;
        
        // Sauvegarde avec retour de succès
        var userData = new { Name = "Test", Email = "test@example.com" };
        var saved = await storage.SaveAsync("user_preferences", userData);
        
        if (saved)
        {
            await "Préférences sauvegardées".ShowSuccessAsync();
        }

        // Chargement avec valeur par défaut
        var defaultPrefs = new { Theme = "Light", Language = "FR" };
        var prefs = await storage.LoadOrDefaultAsync("app_preferences", defaultPrefs);
        
        // Informations sur le fichier
        var fileSize = storage.GetFileSize("user_preferences");
        var lastModified = storage.GetLastModified("user_preferences");
        
        System.Diagnostics.Debug.WriteLine($"Fichier: {fileSize} octets, modifié le {lastModified}");
    }

    /// <summary>
    /// Exemple 6: Validation avec services et extensions string
    /// </summary>
    public static async Task ValidationExample(string email, string password, string firstName)
    {
        // Validation avec messages d'erreur du ValidationService
        var emailError = ValidationService.GetEmailValidationMessage(email);
        if (!string.IsNullOrEmpty(emailError))
        {
            await emailError.ShowErrorAsync();
            return;
        }

        var passwordError = ValidationService.GetPasswordValidationMessage(password);
        if (!string.IsNullOrEmpty(passwordError))
        {
            await passwordError.ShowErrorAsync();
            return;
        }

        var nameError = ValidationService.GetNameValidationMessage(firstName, "prénom");
        if (!string.IsNullOrEmpty(nameError))
        {
            await nameError.ShowErrorAsync();
            return;
        }

        // Formatage avec extensions
        var formattedName = firstName.Capitalize();
        await $"Bienvenue {formattedName}!".ShowSuccessAsync();
    }

    /// <summary>
    /// Exemple 7: Workflow complet de création de session
    /// </summary>
    public static async Task CreateSessionWorkflow(string lockerId, int durationHours, List<string> items)
    {
        var appState = AppStateService.Instance;
        
        // Vérification de l'authentification
        if (!appState.IsUserLoggedIn())
        {
            await "Vous devez être connecté".ShowErrorAsync();
            return;
        }

        // Validation des paramètres
        if (!ValidationService.IsValidLockerId(lockerId))
        {
            await "ID de casier invalide".ShowErrorAsync();
            return;
        }

        if (!ValidationService.IsValidDuration(durationHours))
        {
            await "Durée invalide (1-24h)".ShowErrorAsync();
            return;
        }

        // Création de la session
        var (success, message, session) = await appState.StartSessionWithItemsAsync(lockerId, durationHours, items);
        
        if (success && session != null)
        {
            await NotificationService.Quick.SessionStartedAsync();
            
            // Affichage des détails
            var userName = appState.GetCurrentUserFullName();
            var sessionInfo = $"{userName}, casier {lockerId} pour {durationHours}h";
            await sessionInfo.ShowInfoAsync();
        }
        else
        {
            await (message ?? "Erreur de création de session").ShowErrorAsync();
        }
    }

    /// <summary>
    /// Exemple 8: Gestion des notifications avec actions
    /// </summary>
    public static async Task NotificationWithActionExample()
    {
        // Snackbar avec action personnalisée
        await NotificationService.ShowSnackbarWithActionAsync(
            "Session bientôt expirée",
            "Prolonger",
            async () => {
                // Action de prolongation
                await "Session prolongée".ShowSuccessAsync();
            }
        );
    }

    /// <summary>
    /// Exemple 9: Utilisation des constantes de messages
    /// </summary>
    public static async Task MessageConstantsExample()
    {
        // Utilisation des messages prédéfinis
        await NotificationService.ShowSuccessAsync(NotificationService.Messages.LoginSuccess);
        await NotificationService.ShowErrorAsync(NotificationService.Messages.NetworkError);
        await NotificationService.ShowInfoAsync(NotificationService.Messages.Loading);
        
        // Ou avec les méthodes rapides
        await NotificationService.Quick.AccountCreatedAsync();
        await NotificationService.Quick.ValidationErrorAsync();
    }

    /// <summary>
    /// Exemple 10: Animations de transition de page
    /// </summary>
    public static async Task PageTransitionExample(VisualElement pageContent, bool isEntering)
    {
        // Animation de transition simplifiée
        await AnimationService.PageTransitionAsync(pageContent, isEntering);
        
        // Ou animation personnalisée avec gestion d'erreur
        await pageContent.SafeAnimateAsync(async element => {
            if (isEntering)
            {
                await AnimationService.ScaleInAsync(element);
            }
            else
            {
                await AnimationService.FadeOutAsync(element);
            }
        });
    }
}
