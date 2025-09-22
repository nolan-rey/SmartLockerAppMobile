using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace SmartLockerApp.Services;

/// <summary>
/// Service de notifications simplifié utilisant CommunityToolkit.Maui
/// </summary>
public static class NotificationService
{
    /// <summary>
    /// Affiche un toast de succès
    /// </summary>
    public static async Task ShowSuccessAsync(string message, int durationMs = 3000)
    {
        var toast = Toast.Make(message, ToastDuration.Long);
        await toast.Show();
    }

    /// <summary>
    /// Affiche un toast d'erreur
    /// </summary>
    public static async Task ShowErrorAsync(string message, int durationMs = 4000)
    {
        var toast = Toast.Make(message, ToastDuration.Long);
        await toast.Show();
    }

    /// <summary>
    /// Affiche un toast d'information
    /// </summary>
    public static async Task ShowInfoAsync(string message, int durationMs = 2500)
    {
        var toast = Toast.Make(message, ToastDuration.Short);
        await toast.Show();
    }

    /// <summary>
    /// Affiche un snackbar avec action
    /// </summary>
    public static async Task ShowSnackbarAsync(string message, string actionText = "OK", int durationMs = 3000)
    {
        var snackbar = Snackbar.Make(message, null, actionText, TimeSpan.FromMilliseconds(durationMs));
        await snackbar.Show();
    }

    /// <summary>
    /// Affiche un snackbar avec action personnalisée
    /// </summary>
    public static async Task ShowSnackbarWithActionAsync(
        string message, 
        string actionText, 
        Func<Task> action, 
        int durationMs = 3000)
    {
        var snackbar = Snackbar.Make(message, async () => await action(), actionText, TimeSpan.FromMilliseconds(durationMs));
        await snackbar.Show();
    }


    /// <summary>
    /// Messages prédéfinis pour les opérations courantes
    /// </summary>
    public static class Messages
    {
        // Messages de succès
        public const string LoginSuccess = "Connexion réussie !";
        public const string LogoutSuccess = "Déconnexion réussie !";
        public const string AccountCreated = "Compte créé avec succès !";
        public const string SessionStarted = "Session démarrée !";
        public const string SessionEnded = "Session terminée !";
        public const string LockerLocked = "Casier verrouillé !";
        public const string LockerUnlocked = "Casier déverrouillé !";
        public const string PaymentSuccess = "Paiement effectué !";

        // Messages d'erreur
        public const string LoginError = "Erreur de connexion";
        public const string NetworkError = "Erreur de connexion réseau";
        public const string ValidationError = "Veuillez vérifier vos informations";
        public const string SessionError = "Erreur lors de la session";
        public const string LockerError = "Erreur avec le casier";
        public const string PaymentError = "Erreur de paiement";

        // Messages d'information
        public const string Loading = "Chargement en cours...";
        public const string Processing = "Traitement en cours...";
        public const string Saving = "Sauvegarde...";
        public const string Connecting = "Connexion...";
    }

    /// <summary>
    /// Méthodes rapides pour les messages courants
    /// </summary>
    public static class Quick
    {
        public static async Task LoginSuccessAsync() => 
            await ShowSuccessAsync(Messages.LoginSuccess);

        public static async Task LoginErrorAsync() => 
            await ShowErrorAsync(Messages.LoginError);

        public static async Task LogoutSuccessAsync() => 
            await ShowSuccessAsync(Messages.LogoutSuccess);

        public static async Task AccountCreatedAsync() => 
            await ShowSuccessAsync(Messages.AccountCreated);

        public static async Task SessionStartedAsync() => 
            await ShowSuccessAsync(Messages.SessionStarted);

        public static async Task SessionEndedAsync() => 
            await ShowSuccessAsync(Messages.SessionEnded);

        public static async Task ValidationErrorAsync() => 
            await ShowErrorAsync(Messages.ValidationError);

        public static async Task NetworkErrorAsync() => 
            await ShowErrorAsync(Messages.NetworkError);
    }
}
