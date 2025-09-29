# 🗄️ Guide d'Intégration BDD - SmartLocker App

## ✅ **Fonctionnalités Implémentées**

### **🔐 Création d'Utilisateurs dans la BDD**

L'application enregistre maintenant automatiquement tous les nouveaux comptes utilisateur dans la base de données via l'API SmartLocker.

#### **Flux de Création de Compte :**
1. **SignupPage** → Saisie des informations utilisateur
2. **AppStateService.CreateAccountAsync()** → Orchestration
3. **SmartLockerIntegratedService.CreateUserAsync()** → Service principal
4. **SmartLockerApiService.CreateUserAsync()** → Appel API
5. **Base de données** → Enregistrement permanent

#### **Données Enregistrées :**
- **Username** : Généré automatiquement à partir de l'email (partie avant @)
- **Password** : Haché côté serveur
- **Email** : Email complet de l'utilisateur
- **Name** : Nom complet (prénom + nom)
- **Role** : "user" par défaut

### **📊 Création de Sessions dans la BDD**

Toutes les sessions de casiers sont maintenant enregistrées dans la base de données avec fallback automatique.

#### **Flux de Création de Session :**
1. **DepositSetupPage** → Sélection durée et coût
2. **AppStateService.CreateSessionAsync()** → Orchestration
3. **SmartLockerIntegratedService.CreateSessionAsync()** → Service principal
4. **SmartLockerApiService.CreateSessionAsync()** → Appel API
5. **Base de données** → Enregistrement permanent

#### **Données de Session Enregistrées :**
- **user_id** : ID de l'utilisateur connecté
- **locker_id** : ID numérique du casier (A1→1, B2→2, C3→3)
- **start_time** : Heure de début de session
- **end_time** : Heure de fin prévue
- **cost** : Coût calculé de la session
- **status** : "active" par défaut
- **payment_status** : "pending" par défaut

## 🔧 **Architecture Technique**

### **Services Créés :**

#### **1. SmartLockerIntegratedService**
- **Rôle** : Service principal avec fallback automatique
- **Méthodes clés** :
  - `CreateUserAsync()` : Création d'utilisateur
  - `CreateSessionAsync()` : Création de session
  - `LoginAsync()` : Connexion avec API

#### **2. SmartLockerApiService** 
- **Rôle** : Interface directe avec l'API
- **Méthodes ajoutées** :
  - `CreateUserAsync()` : POST /users
  - `CreateSessionAsync()` : POST /sessions/create

#### **3. AppStateService (Mis à jour)**
- **Rôle** : Orchestration globale
- **Méthodes mises à jour** :
  - `CreateAccountAsync()` : Utilise l'API + fallback local
  - `LoginAsync()` : Authentification API + local
  - `CreateSessionAsync()` : Nouvelle méthode pour sessions BDD

### **DTOs Ajoutés :**
- `CreateUserRequestDto` / `CreateUserResponseDto`
- `CreateSessionRequestDto` / `CreateSessionResponseDto`

## 🚀 **Utilisation**

### **Création de Compte :**
```csharp
// Automatique via SignupPage
var (success, message) = await AppStateService.Instance.CreateAccountAsync(
    email, password, firstName, lastName
);

// Résultat : Utilisateur créé dans la BDD + localement
```

### **Création de Session :**
```csharp
// Nouvelle méthode disponible
var (success, message, session) = await AppStateService.Instance.CreateSessionAsync(
    lockerId: "A1",
    startTime: DateTime.Now,
    endTime: DateTime.Now.AddHours(2),
    cost: 5.00m
);

// Résultat : Session enregistrée dans la BDD + localement
```

## 🔄 **Système de Fallback**

### **Avantages :**
- ✅ **Fonctionnement hors ligne** : L'app continue à marcher sans API
- ✅ **Résilience** : Pas de perte de fonctionnalité en cas de panne
- ✅ **Synchronisation future** : Données locales prêtes pour sync ultérieure

### **Comportement :**
1. **API disponible** → Enregistrement BDD + local
2. **API indisponible** → Enregistrement local uniquement
3. **Erreur API** → Fallback automatique vers local

## 📝 **Messages Utilisateur**

### **Succès API :**
- "Compte créé avec succès dans la BDD !"
- "Session créée avec succès dans la BDD !"

### **Fallback Local :**
- "Compte créé localement (API indisponible)"
- "Session créée localement (mode hors ligne)"

## 🔧 **Configuration**

### **Credentials API :**
- **Base URL** : `https://reymond.alwaysdata.net/smartLockerApi/`
- **Username** : `Smart`
- **Password** : `Locker`

### **Mapping IDs :**
- A1 ↔ 1
- B2 ↔ 2  
- C3 ↔ 3

## ✅ **État Actuel**

- ✅ **Compilation réussie** (17 avertissements mineurs seulement)
- ✅ **Création utilisateurs** → BDD + local
- ✅ **Création sessions** → BDD + local
- ✅ **Authentification** → API + local
- ✅ **Fallback automatique** → Fonctionnel
- ✅ **Compatibilité** → Ancien système préservé

## 🎯 **Prochaines Étapes Possibles**

1. **Synchronisation** : Sync des données locales vers BDD
2. **Historique** : Récupération historique depuis BDD
3. **Paiements** : Enregistrement des transactions
4. **Analytics** : Statistiques depuis BDD
5. **Multi-utilisateurs** : Gestion des comptes partagés

---

**🎉 L'application SmartLocker enregistre maintenant toutes les données importantes dans la base de données tout en conservant une excellente expérience utilisateur !**
