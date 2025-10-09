# 🚀 Parcours Utilisateur - SmartLocker

## ✅ Flux Corrigé (Avec API)

### 📋 Étapes du parcours

#### 1️⃣ **LoginPage** → Connexion utilisateur
- **Action** : L'utilisateur se connecte avec email/mot de passe
- **Backend** : 
  - Obtention du token JWT avec `Smart / Locker`
  - Recherche de l'utilisateur dans l'API par email
  - Sauvegarde de l'utilisateur en local
- **Navigation** : → `HomePage`

---

#### 2️⃣ **HomePage** → Sélection du casier
- **Action** : L'utilisateur clique sur un casier disponible
- **Vérification** : L'utilisateur est connecté (récupéré depuis le stockage local)
- **Navigation** : → `LockerDetailPage?lockerId=1`

---

#### 3️⃣ **LockerDetailPage** → Détails du casier
- **Action** : L'utilisateur clique sur "Utiliser ce casier"
- **Navigation** : → `DepositSetupPage?lockerId=1`

---

#### 4️⃣ **DepositSetupPage** → Sélection de la durée ⚠️ **VÉRIFICATION UTILISATEUR**
- **✅ Vérification** : 
  ```csharp
  var currentUser = await _dataService.GetCurrentUserAsync();
  if (currentUser == null) → Redirection vers LoginPage
  ```
- **Action** : L'utilisateur sélectionne la durée (30min, 1h, 2h, 4h)
- **Paramètres transmis** :
  - `lockerId` : ID du casier (ex: "1")
  - `durationHours` : Durée en heures (ex: "2")
  - `price` : Prix calculé (ex: "7,00")
- **⚠️ PAS DE CRÉATION DE SESSION ICI**
- **Navigation** : → `OpenLockerPage?lockerId=1&durationHours=2&price=7,00`

---

#### 5️⃣ **OpenLockerPage** → Simulation d'ouverture du casier
- **Action** : Simulation de 3 secondes d'ouverture du casier
- **Affichage** : "Casier ouvert ! Vous pouvez maintenant déposer vos affaires"
- **Paramètres transmis** : Tous les paramètres reçus sont passés à la page suivante
- **⚠️ PAS DE CRÉATION DE SESSION ICI**
- **Navigation** : → `LockInstructionsPage?lockerId=1&durationHours=2&price=7,00`

---

#### 6️⃣ **LockInstructionsPage** → Choix de la méthode de verrouillage
- **Action** : L'utilisateur choisit entre RFID ou Empreinte digitale
- **Options** :
  - 📱 **Badge RFID** → `authMethod=rfid`
  - 👆 **Empreinte digitale** → `authMethod=fingerprint`
- **Paramètres transmis** : Tous les paramètres + méthode d'authentification
- **⚠️ PAS DE CRÉATION DE SESSION ICI**
- **Navigation** : → `LockConfirmationPage?lockerId=1&durationHours=2&price=7,00&authMethod=rfid`

---

#### 7️⃣ **LockConfirmationPage** → Confirmation et création de session 🎯 **CRÉATION API**
- **✅ CRÉATION DE LA SESSION DANS L'API** :
  ```csharp
  var currentUser = await _dataService.GetCurrentUserAsync();
  var result = await _dataService.CreateSessionAsync(
      lockerId: "1",
      durationHours: 2,
      items: new List<string>()
  );
  ```
- **Backend API** :
  - `POST /sessions` avec :
    - `user_id` : ID de l'utilisateur connecté
    - `locker_id` : ID du casier
    - `status` : "active"
    - `planned_end_at` : Date/heure de fin calculée
    - `amount_due` : Montant calculé (2.50€/heure)
    - `currency` : "EUR"
    - `payment_status` : "none"
- **Affichage** : "Casier verrouillé avec succès !"
- **Navigation** : → `HomePage` (avec session active visible)

---

## 🔄 Récapitulatif des vérifications

### ✅ Vérifications utilisateur
1. **DepositSetupPage** : Vérification au chargement + avant confirmation
2. **LockConfirmationPage** : Vérification avant création de session

### 🎯 Point de création de session
- **UNIQUEMENT** dans `LockConfirmationPage.OnAppearing()`
- Après que l'utilisateur ait :
  - ✅ Sélectionné un casier
  - ✅ Choisi une durée
  - ✅ Ouvert le casier
  - ✅ Déposé ses affaires
  - ✅ Choisi sa méthode de verrouillage
  - ✅ Verrouillé le casier

## 📊 Données transmises entre les pages

```
HomePage
  ↓ lockerId=1
LockerDetailPage
  ↓ lockerId=1
DepositSetupPage [VÉRIF USER]
  ↓ lockerId=1, durationHours=2, price=7.00
OpenLockerPage
  ↓ lockerId=1, durationHours=2, price=7.00
LockInstructionsPage
  ↓ lockerId=1, durationHours=2, price=7.00, authMethod=rfid
LockConfirmationPage [CRÉATION SESSION API]
  ↓ sessionId=123
HomePage (session active)
```

## 🔧 Services utilisés

### IDataService (ApiDataService)
- `GetCurrentUserAsync()` : Récupère l'utilisateur connecté
- `CreateSessionAsync()` : Crée une session dans l'API

### ApiSessionService
- `CreateSessionAsync()` : Appel API `POST /sessions`

### ApiAuthService
- `GetValidTokenAsync()` : Obtient/renouvelle le token JWT

## ✅ Avantages de cette approche

1. **Sécurité** : L'utilisateur est vérifié dès le début du parcours
2. **Performance** : La session n'est créée qu'une seule fois, à la fin
3. **Cohérence** : Tous les paramètres sont transmis proprement entre les pages
4. **Traçabilité** : Logs détaillés à chaque étape
5. **API First** : Utilisation de l'API réelle pour la création de session

## 🐛 Problèmes résolus

- ❌ **Avant** : Session créée trop tôt dans `DepositSetupPage`
- ✅ **Après** : Session créée au bon moment dans `LockConfirmationPage`

- ❌ **Avant** : Utilisateur non vérifié sur les pages intermédiaires
- ✅ **Après** : Vérification dès `DepositSetupPage`

- ❌ **Avant** : Paramètres perdus entre les pages
- ✅ **Après** : Tous les paramètres transmis proprement via QueryProperty

---

**Dernière mise à jour** : 07/10/2025
