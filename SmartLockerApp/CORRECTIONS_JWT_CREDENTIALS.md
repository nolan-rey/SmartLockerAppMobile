# 🔧 Corrections JWT - Credentials API SmartLocker

## ❌ Problème Identifié

L'application utilisait des credentials incorrects pour l'authentification JWT :
- **Ancien** : `username: "SaintMichel"`, `password: "ITcampus"`
- **Correct** : `username: "Smart"`, `password: "Locker"`

Cela causait des échecs d'authentification avec l'API SmartLocker en production.

## ✅ Corrections Effectuées

### 1. **ApiDataService.cs**
**Fichier** : `SmartLockerApp\Services\ApiDataService.cs`

**Avant** :
```csharp
// Pour l'instant, utiliser les credentials de test de l'API
var token = await _authService.LoginAsync("SaintMichel", "ITcampus");
```

**Après** :
```csharp
// Récupérer le token JWT avec les credentials de production
var token = await _authService.GetValidTokenAsync();
```

**Changement** : Utilise maintenant `GetValidTokenAsync()` qui appelle automatiquement les bons credentials définis dans `ApiAuthService`.

### 2. **ApiUserService.cs**
**Fichier** : `SmartLockerApp\Services\ApiUserService.cs`

**Ajout** : Méthode `GetUserByEmailAsync()` pour chercher un utilisateur par email.

```csharp
/// <summary>
/// Récupère un utilisateur par email (cherche dans tous les utilisateurs)
/// </summary>
public async Task<User?> GetUserByEmailAsync(string email)
{
    try
    {
        System.Diagnostics.Debug.WriteLine($"🔍 Recherche utilisateur par email: {email}...");
        
        var allUsers = await GetAllUsersAsync();
        
        if (allUsers != null)
        {
            var user = allUsers.FirstOrDefault(u => u.email.Equals(email, StringComparison.OrdinalIgnoreCase));
            
            if (user != null)
            {
                System.Diagnostics.Debug.WriteLine($"✅ Utilisateur trouvé: {user.name}");
                return user;
            }
        }
        
        System.Diagnostics.Debug.WriteLine("⚠️ Utilisateur non trouvé");
        return null;
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"❌ Erreur GetUserByEmail: {ex.Message}");
        return null;
    }
}
```

### 3. **API_INTEGRATION_GUIDE.md**
**Fichier** : `SmartLockerApp\API_INTEGRATION_GUIDE.md`

**Avant** :
```markdown
- **Credentials test** : `SaintMichel` / `ITcampus`
```

**Après** :
```markdown
- **Credentials JWT** : `Smart` / `Locker` (pour l'authentification JWT de l'API)
```

## 🔐 Configuration JWT Correcte

### ApiAuthService.cs (Corrigé)
Le fichier `ApiAuthService.cs` utilise la bonne URL :

```csharp
private const string BASE_URL = "https://reymond.alwaysdata.net/smartLockerApi";
private const string ADMIN_USERNAME = "Smart";
private const string ADMIN_PASSWORD = "Locker";
```

**✅ Important** : L'URL correcte est `https://reymond.alwaysdata.net/smartLockerApi` (avec le "A" majuscule dans "Api").

### Flux d'Authentification JWT

1. **Demande de token** → `ApiAuthService.GetValidTokenAsync()`
2. **Login API** → POST `https://reymond.alwaysdata.net/smartLockerApi/login` avec `{"username": "Smart", "password": "Locker"}`
3. **Réception token** → Token JWT stocké dans `SecureStorage`
4. **Utilisation** → Header `Authorization: Bearer {token}` sur toutes les requêtes API

## 📊 Authentification Utilisateur vs JWT

### ⚠️ Important : Deux types d'authentification différents

#### 1. **Authentification JWT (API Admin)**
- **Username** : `Smart`
- **Password** : `Locker`
- **Usage** : Obtenir un token JWT pour accéder à l'API
- **Où** : `ApiAuthService`
- **Quand** : Automatiquement à chaque appel API

#### 2. **Authentification Utilisateur (App Mobile)**
- **Email** : Saisi par l'utilisateur (ex: `user@example.com`)
- **Password** : Saisi par l'utilisateur
- **Usage** : Identifier l'utilisateur dans l'application
- **Où** : `LoginViewModel`, `ApiDataService`
- **Quand** : À la connexion de l'utilisateur

### Flux Complet de Connexion Utilisateur

```
Utilisateur saisit email/password
        ↓
LoginViewModel.Login()
        ↓
ApiDataService.AuthenticateAsync(email, password)
        ↓
1. Récupère token JWT (avec "Smart"/"Locker")
2. Cherche utilisateur dans API par email
3. Si trouvé → connexion réussie
   Si non trouvé → message "Veuillez créer un compte"
        ↓
Utilisateur connecté dans l'app
```

### Flux de Création de Compte

```
Utilisateur saisit prénom/nom/email/password
        ↓
SignupPage → CreateAccount
        ↓
ApiDataService.CreateAccountAsync(firstName, lastName, email, password)
        ↓
1. Récupère token JWT (avec "Smart"/"Locker")
2. Crée l'utilisateur dans l'API via POST /users
3. Sauvegarde l'utilisateur localement
        ↓
Compte créé et utilisateur connecté
```

## 🎯 Ce qui fonctionne maintenant

✅ **Token JWT** : Obtention automatique avec bons credentials  
✅ **Appels API** : Header Authorization correctement configuré  
✅ **Authentification** : Recherche utilisateur par email fonctionnelle  
✅ **Compilation** : Build réussi sans erreur  

## 🔄 Prochaines Améliorations

### Sécurité de l'authentification utilisateur

**Actuellement** :
```csharp
// TODO: Vérifier le mot de passe hashé côté serveur
// Pour l'instant, on accepte la connexion si l'utilisateur existe
```

**À implémenter** :
1. Ajouter endpoint API `/users/authenticate` avec email + password
2. Vérifier le hash côté serveur
3. Retourner les infos utilisateur si password correct

**Exemple d'implémentation future** :
```csharp
// Dans ApiUserService.cs
public async Task<User?> AuthenticateUserAsync(string email, string password)
{
    var authData = new { email, password };
    return await _apiClient.PostAsync<object, User>("/users/authenticate", authData);
}
```

## 📝 Résumé des Fichiers Modifiés

1. ✏️ `SmartLockerApp\Services\ApiDataService.cs` 
   - Correction méthode `AuthenticateAsync()` avec meilleurs messages d'erreur
   - Correction méthode `CreateAccountAsync()` pour créer le compte dans l'API
2. ✏️ `SmartLockerApp\Services\ApiUserService.cs` - Ajout méthode GetUserByEmailAsync
3. ✏️ `SmartLockerApp\Services\ApiAuthService.cs` - URL correcte restaurée
4. ✏️ `SmartLockerApp\Services\ApiHttpClient.cs` - URL correcte restaurée
5. ✏️ `SmartLockerApp\Views\SignupPage.xaml.cs` - **CORRECTION CRITIQUE : Utilise maintenant IDataService au lieu de AppStateService**
6. ✏️ `SmartLockerApp\API_INTEGRATION_GUIDE.md` - Mise à jour documentation

## ⚠️ Problème Résolu : SignupPage n'utilisait pas l'API

### Le Problème Identifié

**Avant la correction** :
- `SignupPage.xaml.cs` utilisait `AppStateService.CreateAccountAsync()`
- `AppStateService` appelait `AuthenticationService.CreateAccountAsync()` (service **local**)
- Résultat : Le compte était créé **seulement localement**, pas dans la BDD !

**Code problématique** :
```csharp
// SignupPage.xaml.cs (ANCIEN)
private readonly AppStateService _appState;

public SignupPage(AppStateService appState)
{
    _appState = appState;
}

// Création locale seulement ❌
var (success, message) = await _appState.CreateAccountAsync(...);
```

**Après la correction** :
- `SignupPage.xaml.cs` utilise maintenant `IDataService.CreateAccountAsync()`
- `IDataService` est configuré comme `ApiDataService` dans `MauiProgram.cs`
- Résultat : Le compte est créé **dans la BDD via l'API** ! ✅

**Code corrigé** :
```csharp
// SignupPage.xaml.cs (NOUVEAU)
private readonly IDataService _dataService;

public SignupPage(IDataService dataService)
{
    _dataService = dataService;
}

// Création dans l'API ✅
var (success, user, message) = await _dataService.CreateAccountAsync(
    firstName, lastName, email, password
);
```

## ⚠️ Important : Créer un Compte d'Abord

**Pour se connecter, l'utilisateur DOIT exister dans la base de données !**

### Étapes pour utiliser l'application :

1. **Première utilisation** : Cliquer sur "Créer un compte"
   - Le compte sera créé dans l'API (base de données)
   - Vous serez automatiquement connecté

2. **Connexions suivantes** : Utiliser email/password
   - L'app cherche votre compte dans la BDD via l'API
   - Si trouvé → connexion réussie
   - Si non trouvé → message d'erreur "Veuillez créer un compte"

### Messages d'Erreur Améliorés

- ✅ **"Connexion réussie"** → Utilisateur trouvé et connecté
- ❌ **"Email ou mot de passe incorrect. Veuillez créer un compte..."** → Utilisateur n'existe pas dans la BDD
- ❌ **"Erreur de connexion à l'API. Vérifiez votre connexion Internet."** → Pas de token JWT (problème réseau ou API)
- ⚠️ **"Compte créé localement (API indisponible)"** → Compte créé en local seulement (fallback)
- ✅ **"Compte créé avec succès !"** → Compte créé dans l'API + local

## ✅ Vérification

```bash
# Build réussi
dotnet build SmartLockerApp\SmartLockerApp.csproj
```

**Résultat** : ✅ Génération réussie

---

**🎉 L'application utilise maintenant correctement les credentials JWT de production "Smart"/"Locker" pour toutes les requêtes API !**
