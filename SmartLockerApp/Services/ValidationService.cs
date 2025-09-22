using System.Text.RegularExpressions;

namespace SmartLockerApp.Services;

/// <summary>
/// Service de validation simplifié utilisant des méthodes d'extension
/// </summary>
public static class ValidationService
{
    // Expressions régulières compilées pour de meilleures performances
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex PasswordRegex = new(
        @"^.{6,}$",
        RegexOptions.Compiled);

    /// <summary>
    /// Valide une adresse email
    /// </summary>
    public static bool IsValidEmail(this string email) =>
        !string.IsNullOrWhiteSpace(email) && EmailRegex.IsMatch(email);

    /// <summary>
    /// Valide un mot de passe (minimum 6 caractères)
    /// </summary>
    public static bool IsValidPassword(this string password) =>
        !string.IsNullOrWhiteSpace(password) && PasswordRegex.IsMatch(password);

    /// <summary>
    /// Valide un nom (non vide, au moins 2 caractères)
    /// </summary>
    public static bool IsValidName(this string name) =>
        !string.IsNullOrWhiteSpace(name) && name.Trim().Length >= 2;

    /// <summary>
    /// Valide un ID de casier
    /// </summary>
    public static bool IsValidLockerId(this string lockerId) =>
        !string.IsNullOrWhiteSpace(lockerId) && lockerId.Length >= 2;

    /// <summary>
    /// Valide une durée de session
    /// </summary>
    public static bool IsValidDuration(this int hours) =>
        hours > 0 && hours <= 24; // Maximum 24 heures

    /// <summary>
    /// Obtient le message d'erreur pour un email invalide
    /// </summary>
    public static string GetEmailValidationMessage(this string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return "L'adresse email est requise";
        
        if (!email.IsValidEmail())
            return "Format d'email invalide";
        
        return string.Empty;
    }

    /// <summary>
    /// Obtient le message d'erreur pour un mot de passe invalide
    /// </summary>
    public static string GetPasswordValidationMessage(this string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return "Le mot de passe est requis";
        
        if (!password.IsValidPassword())
            return "Le mot de passe doit contenir au moins 6 caractères";
        
        return string.Empty;
    }

    /// <summary>
    /// Obtient le message d'erreur pour un nom invalide
    /// </summary>
    public static string GetNameValidationMessage(this string name, string fieldName = "nom")
    {
        if (string.IsNullOrWhiteSpace(name))
            return $"Le {fieldName} est requis";
        
        if (!name.IsValidName())
            return $"Le {fieldName} doit contenir au moins 2 caractères";
        
        return string.Empty;
    }

    /// <summary>
    /// Valide tous les champs de connexion
    /// </summary>
    public static (bool IsValid, List<string> Errors) ValidateLogin(string email, string password)
    {
        var errors = new List<string>();

        var emailError = email.GetEmailValidationMessage();
        if (!string.IsNullOrEmpty(emailError))
            errors.Add(emailError);

        var passwordError = password.GetPasswordValidationMessage();
        if (!string.IsNullOrEmpty(passwordError))
            errors.Add(passwordError);

        return (errors.Count == 0, errors);
    }

    /// <summary>
    /// Valide tous les champs d'inscription
    /// </summary>
    public static (bool IsValid, List<string> Errors) ValidateSignup(
        string firstName, string lastName, string email, string password)
    {
        var errors = new List<string>();

        var firstNameError = firstName.GetNameValidationMessage("prénom");
        if (!string.IsNullOrEmpty(firstNameError))
            errors.Add(firstNameError);

        var lastNameError = lastName.GetNameValidationMessage("nom");
        if (!string.IsNullOrEmpty(lastNameError))
            errors.Add(lastNameError);

        var emailError = email.GetEmailValidationMessage();
        if (!string.IsNullOrEmpty(emailError))
            errors.Add(emailError);

        var passwordError = password.GetPasswordValidationMessage();
        if (!string.IsNullOrEmpty(passwordError))
            errors.Add(passwordError);

        return (errors.Count == 0, errors);
    }
}
