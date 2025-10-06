using System.Security.Cryptography;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using SmartLockerApp.Models;

namespace SmartLockerApp.Services;

/// <summary>
/// Service d'authentification locale avec chiffrement utilisant CommunityToolkit.Mvvm
/// </summary>
public partial class AuthenticationService : ObservableObject
{
    private readonly LocalStorageService _storage;
    private const string UsersKey = "users";
    private const string CurrentUserKey = "current_user";

    private List<UserAccount> _users = new();
    
    [ObservableProperty]
    private UserAccount? _currentUser;

    public bool IsAuthenticated => CurrentUser != null;

    public AuthenticationService(LocalStorageService storage)
    {
        _storage = storage;
        _ = InitializeAsync();
    }

    /// <summary>
    /// Initialise le service et crée un utilisateur de test si nécessaire
    /// </summary>
    private async Task InitializeAsync()
    {
        await LoadUsersAsync();
        await CreateDefaultUserIfNeededAsync();
    }

    /// <summary>
    /// Crée un utilisateur de test par défaut si aucun utilisateur n'existe
    /// </summary>
    private async Task CreateDefaultUserIfNeededAsync()
    {
        if (_users.Count == 0)
        {
            var defaultUser = new UserAccount
            {
                Id = Guid.NewGuid().ToString(),
                Email = "test@smartlocker.com",
                FirstName = "Utilisateur",
                LastName = "Test",
                PasswordHash = HashPassword("123456"),
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            _users.Add(defaultUser);
            await _storage.SaveAsync(UsersKey, _users);
            
            System.Diagnostics.Debug.WriteLine("Utilisateur de test créé:");
            System.Diagnostics.Debug.WriteLine("Email: test@smartlocker.com");
            System.Diagnostics.Debug.WriteLine("Mot de passe: 123456");
        }
    }

    /// <summary>
    /// Charge les utilisateurs depuis le stockage local
    /// </summary>
    private async Task LoadUsersAsync()
    {
        _users = await _storage.LoadAsync<List<UserAccount>>(UsersKey) ?? new List<UserAccount>();
        var currentUserId = await _storage.LoadAsync<string>(CurrentUserKey);
        if (!string.IsNullOrEmpty(currentUserId))
        {
            CurrentUser = _users.FirstOrDefault(u => u.Id == currentUserId);
        }
    }

    /// <summary>
    /// Crée un nouveau compte utilisateur
    /// </summary>
    public async Task<(bool Success, string Message)> CreateAccountAsync(string email, string password, string firstName, string lastName)
    {
        try
        {
            // Validation
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return (false, "Email et mot de passe requis");

            if (password.Length < 6)
                return (false, "Le mot de passe doit contenir au moins 6 caractères");

            if (_users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
                return (false, "Un compte avec cet email existe déjà");

            // Création du compte
            var user = new UserAccount
            {
                Id = Guid.NewGuid().ToString(),
                Email = email.ToLowerInvariant(),
                FirstName = firstName,
                LastName = lastName,
                PasswordHash = HashPassword(password),
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            _users.Add(user);
            await _storage.SaveAsync(UsersKey, _users);

            return (true, "Compte créé avec succès");
        }
        catch (Exception ex)
        {
            return (false, $"Erreur lors de la création du compte: {ex.Message}");
        }
    }

    /// <summary>
    /// Connexion utilisateur
    /// </summary>
    public async Task<(bool Success, string Message)> LoginAsync(string email, string password)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return (false, "Email et mot de passe requis");

            var user = _users.FirstOrDefault(u => 
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && u.IsActive);

            if (user == null)
                return (false, "Email ou mot de passe incorrect");

            if (!VerifyPassword(password, user.PasswordHash))
                return (false, "Email ou mot de passe incorrect");

            CurrentUser = user;
            user.LastLoginAt = DateTime.Now;
            
            await _storage.SaveAsync(UsersKey, _users);
            await _storage.SaveAsync(CurrentUserKey, user.Id);

            return (true, "Connexion réussie");
        }
        catch (Exception ex)
        {
            return (false, $"Erreur de connexion: {ex.Message}");
        }
    }

    /// <summary>
    /// Déconnexion
    /// </summary>
    public async Task LogoutAsync()
    {
        CurrentUser = null;
        _storage.Delete(CurrentUserKey);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Met à jour les informations utilisateur
    /// </summary>
    public async Task<bool> UpdateUserAsync(UserAccount updatedUser)
    {
        try
        {
            var userIndex = _users.FindIndex(u => u.Id == updatedUser.Id);
            if (userIndex >= 0)
            {
                _users[userIndex] = updatedUser;
                if (CurrentUser?.Id == updatedUser.Id)
                    CurrentUser = updatedUser;

                await _storage.SaveAsync(UsersKey, _users);
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Hache le mot de passe
    /// </summary>
    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var salt = "SmartLocker2024"; // En production, utiliser un salt unique par utilisateur
        var saltedPassword = password + salt;
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
        return Convert.ToBase64String(hashedBytes);
    }

    /// <summary>
    /// Vérifie le mot de passe
    /// </summary>
    private static bool VerifyPassword(string password, string hash)
    {
        return HashPassword(password) == hash;
    }
}

/// <summary>
/// Modèle de compte utilisateur
/// </summary>
public class UserAccount
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
    public string FullName => $"{FirstName} {LastName}".Trim();
}
