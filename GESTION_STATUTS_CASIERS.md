# 🔒 Gestion des Statuts de Casiers

## ✅ Problème résolu

**Avant** : Quand une session était créée, le casier restait en statut `"available"`, permettant de créer plusieurs sessions sur le même casier.

**Après** : Le statut du casier est automatiquement mis à jour lors de la création et de la fin d'une session.

---

## 🔄 Cycle de vie d'un casier

### 1️⃣ **Casier disponible** (`"available"`)
```
État initial : Le casier est libre et peut être réservé
```

### 2️⃣ **Création de session** → Casier occupé (`"occupied"`)
```csharp
// Dans ApiDataService.CreateSessionAsync()

// 1. Créer la session dans l'API
var (success, message, session) = await _sessionService.CreateSessionAsync(...);

// 2. Mettre à jour le statut du casier
if (success && session != null)
{
    await _lockerService.SetLockerOccupiedAsync(lockerIdInt);
    // Casier maintenant en "occupied"
}
```

**Logs attendus** :
```
✅ Session créée avec succès: ID 123
🔒 Mise à jour du statut du casier 1 en 'occupied'...
✅ Casier 1 marqué comme occupé
```

### 3️⃣ **Fin de session** → Casier disponible (`"available"`)
```csharp
// Dans ApiDataService.EndSessionAsync()

// 1. Récupérer la session pour obtenir le locker_id
var session = await _sessionService.GetSessionByIdAsync(id);

// 2. Terminer la session
var (success, message) = await _sessionService.UpdateSessionAsync(id, "finished", endedAt);

// 3. Libérer le casier
if (success)
{
    await _lockerService.SetLockerAvailableAsync(session.LockerId);
    // Casier maintenant en "available"
}
```

**Logs attendus** :
```
🔚 Fin de session: 123
✅ Session trouvée: Casier 1
✅ Session 123 terminée
🔓 Libération du casier 1...
✅ Casier 1 marqué comme disponible
```

---

## 📊 Statuts possibles d'un casier

| Statut | Description | Peut créer session ? |
|--------|-------------|---------------------|
| `"available"` | Casier libre | ✅ Oui |
| `"occupied"` | Casier en cours d'utilisation | ❌ Non |
| `"maintenance"` | Casier en maintenance | ❌ Non |
| `"out_of_order"` | Casier hors service | ❌ Non |

---

## 🔧 Méthodes API utilisées

### ApiLockerService

#### Mettre un casier en "occupied"
```csharp
public async Task<(bool Success, string Message)> SetLockerOccupiedAsync(int lockerId)
{
    return await SetLockerStatusAsync(lockerId, "occupied");
}
```

**Appel API** : `PUT /lockers/{id}` avec `{"status": "occupied"}`

#### Mettre un casier en "available"
```csharp
public async Task<(bool Success, string Message)> SetLockerAvailableAsync(int lockerId)
{
    return await SetLockerStatusAsync(lockerId, "available");
}
```

**Appel API** : `PUT /lockers/{id}` avec `{"status": "available"}`

---

## 🎯 Points d'intégration

### 1. Création de session
**Fichier** : `ApiDataService.cs`
**Méthode** : `CreateSessionAsync()`
**Ligne** : ~300-315

```csharp
// Après création de session réussie
var (lockerUpdateSuccess, lockerUpdateMessage) = 
    await _lockerService.SetLockerOccupiedAsync(lockerIdInt);
```

### 2. Fin de session
**Fichier** : `ApiDataService.cs`
**Méthode** : `EndSessionAsync()`
**Ligne** : ~418-430

```csharp
// Après fin de session réussie
var (lockerUpdateSuccess, lockerUpdateMessage) = 
    await _lockerService.SetLockerAvailableAsync(session.LockerId);
```

---

## ✅ Avantages de cette approche

1. **Cohérence** : Le statut du casier reflète toujours l'état réel
2. **Prévention** : Impossible de créer deux sessions sur le même casier
3. **Traçabilité** : Logs détaillés de chaque changement de statut
4. **Robustesse** : Si la mise à jour du casier échoue, la session est quand même créée/terminée
5. **API First** : Utilise l'API réelle pour la gestion des statuts

---

## 🧪 Test du cycle complet

### Scénario de test

1. **État initial** : Casier 1 en `"available"`
2. **Créer une session** sur le casier 1
   - ✅ Session créée
   - ✅ Casier 1 passe en `"occupied"`
3. **Essayer de créer une autre session** sur le casier 1
   - ❌ Devrait échouer (casier occupé)
4. **Terminer la session**
   - ✅ Session terminée
   - ✅ Casier 1 repasse en `"available"`
5. **Créer une nouvelle session** sur le casier 1
   - ✅ Devrait fonctionner (casier disponible)

### Logs attendus

```
📝 Création de session:
   - Casier ID: 1
   - Durée: 2 heure(s)
✅ Utilisateur connecté: Test User (ID: 1)
✅ Session créée avec succès: ID 123
🔒 Mise à jour du statut du casier 1 en 'occupied'...
📤 PUT lockers/1
📤 Request body JSON: {"status":"occupied"}
📥 Response status: OK (200)
✅ Casier 1 marqué comme occupé

[... plus tard ...]

🔚 Fin de session: 123
✅ Session trouvée: Casier 1
✅ Session 123 terminée
🔓 Libération du casier 1...
📤 PUT lockers/1
📤 Request body JSON: {"status":"available"}
📥 Response status: OK (200)
✅ Casier 1 marqué comme disponible
```

---

## 🐛 Gestion d'erreurs

### Si la mise à jour du casier échoue

**Lors de la création** :
- ⚠️ La session est quand même créée
- ⚠️ Log d'avertissement affiché
- ⚠️ L'utilisateur peut continuer

**Lors de la fin** :
- ⚠️ La session est quand même terminée
- ⚠️ Log d'avertissement affiché
- ⚠️ Le casier devra être libéré manuellement ou via un job de nettoyage

### Raisons possibles d'échec
- 🌐 Perte de connexion internet
- 🔒 Token JWT expiré
- 🚫 Permissions insuffisantes
- 🐛 Erreur serveur API

---

**Dernière mise à jour** : 09/10/2025
