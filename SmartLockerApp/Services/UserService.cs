using System.Security.Cryptography;
using System.Text;
using SmartLockerApp.Models;

namespace SmartLockerApp.Services;

/// <summary>
/// Service de gestion des utilisateurs (CRUD complet)
/// Implémente toutes les méthodes de l'API Users en local
/// </summary>
public class UserService
{
    private readonly LocalStorageService _storage;
    private const string USERS_KEY = "users";
    private List<User> _users = new();
    private int _nextId = 1;

    public UserService(LocalStorageService storage)
    {
        _storage = storage;
        _ = InitializeAsync();
    }

    /// <summary>
    /// Initialise le service et charge les utilisateurs
    /// </summary>
    private async Task InitializeAsync()
    {
        _users = await _storage.LoadAsync<List<User>>(USERS_KEY) ?? new List<User>();
        
        if (_users.Count > 0)
        {
            _nextId = _users.Max(u => u.id) + 1;
        }
        else
        {
            // Créer des utilisateurs de test si aucun n'existe
            await CreateDefaultUsersAsync();
        }
    }

    /// <summary>
    /// Crée des utilisateurs de test par défaut
    /// </summary>
    private async Task CreateDefaultUsersAsync()
    {
        var defaultUsers = new[]
        {
            new User
            {
                id = _nextId++,
                name = "Smart Locker",
                email = "smart@locker.com",
                password_hash = HashPassword("Locker"),
                role = "admin",
                created_at = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            },
            new User
            {
                id = _nextId++,
                name = "Test User",
                email = "test@smartlocker.com",
                password_hash = HashPassword("123456"),
                role = "user",
                created_at = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            }
        };

        _users.AddRange(defaultUsers);
        await SaveUsersAsync();
    }

    /// <summary>
    /// Hache un mot de passe avec SHA256
    /// </summary>
    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Vérifie si un mot de passe correspond au hash
    /// </summary>
    public bool VerifyPassword(string password, string passwordHash)
    {
        var hash = HashPassword(password);
        return hash == passwordHash;
    }

    /// <summary>
    /// Sauvegarde les utilisateurs dans le stockage local
    /// </summary>
    private async Task<bool> SaveUsersAsync()
    {
        return await _storage.SaveAsync(USERS_KEY, _users);
    }

    #region GET - Récupération

    /// <summary>
    /// GET /users - Récupère tous les utilisateurs
    /// </summary>
    public async Task<List<User>> GetAllUsersAsync()
    {
        // Recharger depuis le stockage pour avoir les données à jour
        _users = await _storage.LoadAsync<List<User>>(USERS_KEY) ?? new List<User>();
        
        // Retourner une copie pour éviter les modifications directes
        return _users.Select(u => new User
        {
            id = u.id,
            name = u.name,
            email = u.email,
            password_hash = u.password_hash,
            role = u.role,
            created_at = u.created_at
        }).ToList();
    }

    /// <summary>
    /// GET /users/{id} - Récupère un utilisateur par ID
    /// </summary>
    public async Task<User?> GetUserByIdAsync(int userId)
    {
        _users = await _storage.LoadAsync<List<User>>(USERS_KEY) ?? new List<User>();
        
        var user = _users.FirstOrDefault(u => u.id == userId);
        
        if (user == null)
            return null;

        // Retourner une copie
        return new User
        {
            id = user.id,
            name = user.name,
            email = user.email,
            password_hash = user.password_hash,
            role = user.role,
            created_at = user.created_at
        };
    }

    /// <summary>
    /// Récupère un utilisateur par email
    /// </summary>
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        _users = await _storage.LoadAsync<List<User>>(USERS_KEY) ?? new List<User>();
        
        var user = _users.FirstOrDefault(u => 
            u.email.Equals(email, StringComparison.OrdinalIgnoreCase));
        
        if (user == null)
            return null;

        return new User
        {
            id = user.id,
            name = user.name,
            email = user.email,
            password_hash = user.password_hash,
            role = user.role,
            created_at = user.created_at
        };
    }

    #endregion

    #region POST - Création

    /// <summary>
    /// POST /users - Crée un nouvel utilisateur
    /// Paramètres : name, email, password_hash, role
    /// </summary>
    public async Task<(bool Success, string Message, User? User)> CreateUserAsync(
        string name, 
        string email, 
        string password, 
        string role = "user")
    {
        try
        {
            // Validation
            if (string.IsNullOrWhiteSpace(name))
                return (false, "Le nom est requis", null);

            if (string.IsNullOrWhiteSpace(email))
                return (false, "L'email est requis", null);

            if (string.IsNullOrWhiteSpace(password))
                return (false, "Le mot de passe est requis", null);

            // Recharger les utilisateurs
            _users = await _storage.LoadAsync<List<User>>(USERS_KEY) ?? new List<User>();

            // Vérifier si l'email existe déjà
            if (_users.Any(u => u.email.Equals(email, StringComparison.OrdinalIgnoreCase)))
                return (false, "Cet email est déjà utilisé", null);

            // Mettre à jour le prochain ID
            if (_users.Count > 0)
                _nextId = _users.Max(u => u.id) + 1;

            // Créer le nouvel utilisateur
            var newUser = new User
            {
                id = _nextId++,
                name = name,
                email = email.ToLower(),
                password_hash = HashPassword(password),
                role = role,
                created_at = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            _users.Add(newUser);
            await SaveUsersAsync();

            System.Diagnostics.Debug.WriteLine($"✅ Utilisateur créé: ID={newUser.id}, Name={newUser.name}, Email={newUser.email}");

            return (true, "Utilisateur créé avec succès", newUser);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur CreateUser: {ex.Message}");
            return (false, $"Erreur lors de la création: {ex.Message}", null);
        }
    }

    #endregion

    #region PUT - Mise à jour

    /// <summary>
    /// PUT /users/{id} - Met à jour un utilisateur
    /// Paramètres : name, email, role
    /// </summary>
    public async Task<(bool Success, string Message, User? User)> UpdateUserAsync(
        int userId,
        string? name = null,
        string? email = null,
        string? role = null)
    {
        try
        {
            // Recharger les utilisateurs
            _users = await _storage.LoadAsync<List<User>>(USERS_KEY) ?? new List<User>();

            var user = _users.FirstOrDefault(u => u.id == userId);
            
            if (user == null)
                return (false, "Utilisateur introuvable", null);

            // Mise à jour des champs fournis
            if (!string.IsNullOrWhiteSpace(name))
                user.name = name;

            if (!string.IsNullOrWhiteSpace(email))
            {
                // Vérifier que l'email n'est pas déjà utilisé par un autre utilisateur
                if (_users.Any(u => u.id != userId && 
                    u.email.Equals(email, StringComparison.OrdinalIgnoreCase)))
                {
                    return (false, "Cet email est déjà utilisé", null);
                }
                user.email = email.ToLower();
            }

            if (!string.IsNullOrWhiteSpace(role))
                user.role = role;

            await SaveUsersAsync();

            System.Diagnostics.Debug.WriteLine($"✅ Utilisateur mis à jour: ID={user.id}");

            return (true, "Utilisateur mis à jour avec succès", user);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur UpdateUser: {ex.Message}");
            return (false, $"Erreur lors de la mise à jour: {ex.Message}", null);
        }
    }

    /// <summary>
    /// Met à jour le mot de passe d'un utilisateur
    /// </summary>
    public async Task<(bool Success, string Message)> UpdatePasswordAsync(
        int userId,
        string oldPassword,
        string newPassword)
    {
        try
        {
            _users = await _storage.LoadAsync<List<User>>(USERS_KEY) ?? new List<User>();

            var user = _users.FirstOrDefault(u => u.id == userId);
            
            if (user == null)
                return (false, "Utilisateur introuvable");

            // Vérifier l'ancien mot de passe
            if (!VerifyPassword(oldPassword, user.password_hash))
                return (false, "Ancien mot de passe incorrect");

            // Valider le nouveau mot de passe
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                return (false, "Le nouveau mot de passe doit contenir au moins 6 caractères");

            // Mettre à jour le mot de passe
            user.password_hash = HashPassword(newPassword);
            await SaveUsersAsync();

            System.Diagnostics.Debug.WriteLine($"✅ Mot de passe mis à jour pour l'utilisateur ID={userId}");

            return (true, "Mot de passe mis à jour avec succès");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur UpdatePassword: {ex.Message}");
            return (false, $"Erreur lors de la mise à jour du mot de passe: {ex.Message}");
        }
    }

    #endregion

    #region DELETE - Suppression

    /// <summary>
    /// DELETE /users/{id} - Supprime un utilisateur
    /// </summary>
    public async Task<(bool Success, string Message)> DeleteUserAsync(int userId)
    {
        try
        {
            _users = await _storage.LoadAsync<List<User>>(USERS_KEY) ?? new List<User>();

            var user = _users.FirstOrDefault(u => u.id == userId);
            
            if (user == null)
                return (false, "Utilisateur introuvable");

            _users.Remove(user);
            await SaveUsersAsync();

            System.Diagnostics.Debug.WriteLine($"✅ Utilisateur supprimé: ID={userId}");

            return (true, "Utilisateur supprimé avec succès");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur DeleteUser: {ex.Message}");
            return (false, $"Erreur lors de la suppression: {ex.Message}");
        }
    }

    #endregion

    #region Authentification

    /// <summary>
    /// Authentifie un utilisateur avec email et mot de passe
    /// </summary>
    public async Task<(bool Success, string Message, User? User)> AuthenticateAsync(
        string email, 
        string password)
    {
        try
        {
            var user = await GetUserByEmailAsync(email);

            if (user == null)
                return (false, "Email ou mot de passe incorrect", null);

            if (!VerifyPassword(password, user.password_hash))
                return (false, "Email ou mot de passe incorrect", null);

            System.Diagnostics.Debug.WriteLine($"✅ Authentification réussie: {user.name} ({user.email})");

            return (true, "Authentification réussie", user);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur Authenticate: {ex.Message}");
            return (false, "Erreur lors de l'authentification", null);
        }
    }

    #endregion

    #region Utilitaires

    /// <summary>
    /// Compte le nombre total d'utilisateurs
    /// </summary>
    public async Task<int> GetUserCountAsync()
    {
        _users = await _storage.LoadAsync<List<User>>(USERS_KEY) ?? new List<User>();
        return _users.Count;
    }

    /// <summary>
    /// Vérifie si un email existe déjà
    /// </summary>
    public async Task<bool> EmailExistsAsync(string email)
    {
        _users = await _storage.LoadAsync<List<User>>(USERS_KEY) ?? new List<User>();
        return _users.Any(u => u.email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Récupère tous les utilisateurs avec un rôle spécifique
    /// </summary>
    public async Task<List<User>> GetUsersByRoleAsync(string role)
    {
        _users = await _storage.LoadAsync<List<User>>(USERS_KEY) ?? new List<User>();
        return _users.Where(u => u.role.Equals(role, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    /// <summary>
    /// Recherche des utilisateurs par nom ou email
    /// </summary>
    public async Task<List<User>> SearchUsersAsync(string searchTerm)
    {
        _users = await _storage.LoadAsync<List<User>>(USERS_KEY) ?? new List<User>();
        
        return _users.Where(u =>
            u.name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            u.email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
        ).ToList();
    }

    #endregion
}
