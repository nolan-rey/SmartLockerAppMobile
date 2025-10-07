# Guide d'Intégration API SmartLocker

## ✅ Ce qui est prêt et fonctionnel

### Services API créés
- **`ApiHttpService`** : Client HTTP simple avec JWT automatique
- **`SmartLockerApiService`** : Service d'intégration avec toutes les fonctionnalités API
- **`SmartLockerIntegratedService`** : Service principal avec fallback automatique
- **`ApiMappingService`** : Conversion DTOs ↔ Models
- **`ApiTestService`** : Tests d'intégration
- **`CompatibilityService`** : Helpers pour migration

### DTOs créés
- `LoginRequestDto` / `LoginResponseDto`
- `UserDto` / `CreateUserDto` / `UpdateUserDto`
- `LockerDto` / `CreateLockerDto` / `UpdateLockerDto`
- `SessionDto` / `StartSessionDto` / `CloseSessionDto`
- `AuthMethodDto` / `SessionAuthDto`

### Modèles adaptés
- **`User`** : ID int, Name string (compatible API)
- **`Locker`** : ID int, Status string (compatible API)
- **`LockerSession`** : IDs int, Status string (compatible API)

### Configuration
- **Base URL** : `https://reymond.alwaysdata.net/smartLockerApi/`
- **Credentials JWT** : `Smart` / `Locker` (pour l'authentification JWT de l'API)
- **Injection de dépendances** : configurée dans `MauiProgram.cs`

## 🚀 Comment utiliser l'API dans tes ViewModels

### 1. Injection de dépendance simple

```csharp
public class MonViewModel : ObservableObject
{
    private readonly SmartLockerIntegratedService _smartLockerService;

    public MonViewModel(SmartLockerIntegratedService smartLockerService)
    {
        _smartLockerService = smartLockerService;
    }
}
```

### 2. Authentification

```csharp
// Connexion avec credentials de test
var success = await _smartLockerService.LoginAsync();

// Connexion avec credentials personnalisés
var success = await _smartLockerService.LoginAsync("username", "password");

// Vérifier si connecté
bool isConnected = _smartLockerService.IsAuthenticated();

// Se déconnecter
_smartLockerService.Logout();
```

### 3. Gestion des casiers

```csharp
// Récupérer les casiers disponibles
var lockers = await _smartLockerService.GetAvailableLockersAsync();

// Récupérer un casier spécifique
var locker = await _smartLockerService.GetLockerAsync(1);

// Ouvrir un casier
var success = await _smartLockerService.OpenLockerAsync(1);
```

### 4. Gestion des sessions

```csharp
// Démarrer une session
var session = await _smartLockerService.StartSessionAsync(
    userId: 1, 
    lockerId: 1, 
    plannedEndAt: DateTime.Now.AddHours(2)
);

// Récupérer les sessions actives
var sessions = await _smartLockerService.GetActiveSessionsAsync();

// Clôturer une session
var success = await _smartLockerService.CloseSessionAsync(sessionId, "paid");
```

### 5. Test de l'API

```csharp
// Test complet avec rapport détaillé
var report = await _smartLockerService.TestApiConnectionAsync();
Console.WriteLine(report);

// Vérifier si l'API est disponible
bool isApiAvailable = _smartLockerService.IsApiAvailable;
```

## 📝 Exemple complet : ApiDemoViewModel

Un ViewModel d'exemple complet est disponible dans `ViewModels/ApiDemoViewModel.cs` qui montre :
- Connexion/déconnexion
- Chargement des casiers
- Gestion des sessions
- Tests API
- Gestion des erreurs

## 🔄 Fallback automatique

Le `SmartLockerIntegratedService` gère automatiquement :
- **API disponible** : utilise l'API réelle
- **API indisponible** : bascule vers des données locales de test
- **Transition transparente** : aucun changement de code nécessaire

## ⚠️ État actuel

### ✅ Fonctionnel
- Architecture API complète
- Services d'intégration
- DTOs et mapping
- Tests de connexion
- Fallback automatique

### 🔧 En cours
- Correction des anciens services (LockerManagementService, etc.)
- Migration complète des ViewModels existants

### 💡 Recommandation

**Pour les nouveaux développements** : utilise directement `SmartLockerIntegratedService`

**Pour l'existant** : migration progressive en remplaçant les anciens services

## 🧪 Test rapide

```csharp
// Dans n'importe quel ViewModel ou service
var apiService = new SmartLockerIntegratedService(new SmartLockerApiService());
var success = await apiService.LoginAsync();
if (success)
{
    var lockers = await apiService.GetAvailableLockersAsync();
    Console.WriteLine($"API fonctionne ! {lockers.Count} casiers trouvés");
}
```

L'API est **prête à être utilisée** ! 🎉
