# 🔌 Intégration API SmartLocker

## ✅ Configuration Actuelle

L'application SmartLocker est maintenant **configurée pour utiliser l'API** !

### 📡 Informations API

- **Base URL** : `https://reymond.alwaysdata.net/smartLockerApi`
- **Credentials de test** : 
  - Username : `SaintMichel`
  - Password : `ITcampus`
- **Authentification** : JWT (Bearer Token)
- **Timeout** : 30 secondes

## 🏗️ Architecture

### Services API Créés

1. **ApiAuthService** - Authentification JWT
   - Gestion automatique du token
   - Renouvellement automatique si expiré
   - Credentials admin intégrés

2. **ApiHttpClient** - Client HTTP avec JWT
   - Injection automatique du Bearer Token
   - Gestion des requêtes GET/POST/PUT/DELETE
   - Timeout et retry configurables

3. **ApiUserService** - Gestion des utilisateurs
4. **ApiLockerService** - Gestion des casiers
5. **ApiSessionService** - Gestion des sessions
6. **ApiAuthMethodService** - Méthodes d'authentification
7. **ApiSessionAuthService** - Liaisons session/auth

### Service de Données

**ApiDataService** implémente `IDataService` et fait le pont entre l'API et l'application :

- ✅ Conversion automatique entre modèles API (`Session`) et modèles App (`LockerSession`)
- ✅ Gestion des erreurs avec logging
- ✅ Compatible avec tous les ViewModels existants
- ✅ Service de mapping `SessionMappingService` pour les conversions

## 🔄 Basculer entre API et Données Locales

Dans `/SmartLockerApp/MauiProgram.cs` ligne 44-45 :

```csharp
// ✅ ACTUEL : Utilise l'API
builder.Services.AddSingleton<IDataService, ApiDataService>();

// Pour revenir aux données locales :
// builder.Services.AddSingleton<IDataService, LocalDataService>();
```

## 📋 Fonctionnalités API Disponibles

### Authentification
- ✅ Login avec JWT automatique
- ✅ Création de compte (stockage local pour l'instant)
- ✅ Gestion de session utilisateur

### Casiers
- ✅ Récupération des casiers disponibles
- ✅ Détails d'un casier spécifique
- ✅ Mise à jour du statut (ouvert/fermé)

### Sessions
- ✅ Création de session
- ✅ Récupération de la session active
- ✅ Historique des sessions
- ✅ Fin de session
- ✅ Mise à jour de session

### Tarification
- ✅ Calcul du prix (2.50€/heure)

## 🧪 Test de l'API

L'application se connecte automatiquement à l'API au démarrage avec les credentials de test.

### Vérifier la connexion

Lors du login, l'application :
1. S'authentifie auprès de l'API avec `SaintMichel / ITcampus`
2. Récupère un token JWT
3. Utilise ce token pour toutes les requêtes suivantes

### Logs de débogage

Les logs sont visibles dans la console de débogage :
- ✅ `✅ Token JWT valide`
- ✅ `✅ Connexion réussie`
- ❌ `❌ Erreur login API: ...`

## 🔧 Modèles et Mapping

### Modèles API (format JSON)

**Session** (API) :
```json
{
  "id": 1,
  "user_id": 1,
  "locker_id": 1,
  "status": "active",
  "started_at": "2025-10-06 10:00:00",
  "planned_end_at": "2025-10-06 12:00:00",
  "ended_at": null,
  "amount_due": "5.00",
  "currency": "EUR",
  "payment_status": "none"
}
```

### Modèles App

**LockerSession** (App) :
```csharp
{
  Id = 1,
  UserId = 1,
  LockerId = 1,
  Status = "active",
  StartedAt = DateTime,
  PlannedEndAt = DateTime,
  EndedAt = null,
  AmountDue = 5.00m,
  Currency = "EUR",
  PaymentStatus = "none"
}
```

### Conversion Automatique

Le service `SessionMappingService` gère automatiquement :
- `Session.ToLockerSession()` - API → App
- `LockerSession.ToSession()` - App → API
- `List<Session>.ToLockerSessions()` - Liste API → Liste App

## 📝 Utilisation dans les ViewModels

Les ViewModels utilisent `IDataService` sans savoir s'il s'agit de l'API ou des données locales :

```csharp
public class MonViewModel : BaseViewModel
{
    private readonly IDataService _dataService;

    public MonViewModel(IDataService dataService)
    {
        _dataService = dataService;
    }

    private async Task LoadDataAsync()
    {
        // Récupère les casiers (API ou local selon config)
        var lockers = await _dataService.GetAvailableLockersAsync();
        
        // Crée une session (API ou local selon config)
        var (success, session, message) = await _dataService.CreateSessionAsync(
            lockerId: "1",
            durationHours: 2,
            items: new List<string>()
        );
    }
}
```

## 🚀 Prochaines Étapes

### Améliorations Possibles

1. **Authentification Utilisateur Réelle**
   - Actuellement : utilise credentials admin pour tous
   - À faire : authentification par utilisateur individuel

2. **Fallback Automatique**
   - Détecter si l'API est indisponible
   - Basculer automatiquement vers données locales
   - Synchroniser quand l'API revient

3. **Cache Local**
   - Mettre en cache les données API
   - Réduire les appels réseau
   - Mode hors ligne

4. **Gestion d'Erreurs Améliorée**
   - Messages d'erreur plus détaillés
   - Retry automatique
   - Indicateurs de chargement

## 🐛 Dépannage

### L'API ne répond pas

1. Vérifier la connexion internet
2. Vérifier que l'URL est accessible : `https://reymond.alwaysdata.net/smartLockerApi`
3. Consulter les logs dans la console de débogage

### Erreur d'authentification

1. Vérifier les credentials dans `ApiAuthService.cs`
2. Le token JWT expire après 1 heure (renouvellement automatique)

### Basculer vers données locales

Dans `MauiProgram.cs`, commenter `ApiDataService` et décommenter `LocalDataService`

## 📊 État Actuel

- ✅ **API configurée et fonctionnelle**
- ✅ **Services API créés et injectés**
- ✅ **Mapping automatique Session ↔ LockerSession**
- ✅ **Compatible avec tous les ViewModels**
- ✅ **Compilation sans erreurs**
- 🚀 **Prêt pour les tests !**

---

**Dernière mise à jour** : 06/10/2025
