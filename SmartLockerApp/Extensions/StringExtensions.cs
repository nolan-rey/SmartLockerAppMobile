using SmartLockerApp.Services;

namespace SmartLockerApp.Extensions;

/// <summary>
/// Extensions pour les chaînes de caractères
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Affiche un message de succès pour une opération
    /// </summary>
    public static async Task ShowSuccessAsync(this string message) =>
        await NotificationService.ShowSuccessAsync(message);

    /// <summary>
    /// Affiche un message d'erreur pour une opération
    /// </summary>
    public static async Task ShowErrorAsync(this string message) =>
        await NotificationService.ShowErrorAsync(message);

    /// <summary>
    /// Affiche un message d'information
    /// </summary>
    public static async Task ShowInfoAsync(this string message) =>
        await NotificationService.ShowInfoAsync(message);


    /// <summary>
    /// Capitalise la première lettre d'une chaîne
    /// </summary>
    public static string Capitalize(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        return char.ToUpper(input[0]) + input[1..].ToLower();
    }

    /// <summary>
    /// Tronque une chaîne à la longueur spécifiée avec des points de suspension
    /// </summary>
    public static string Truncate(this string input, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(input) || input.Length <= maxLength)
            return input;

        return input[..(maxLength - 3)] + "...";
    }
}
