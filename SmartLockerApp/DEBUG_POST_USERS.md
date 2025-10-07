# 🔧 Debug POST /users - Logs Améliorés

## ✅ Token JWT Fonctionne !

Le problème du token JWT est résolu. Maintenant on debug la création d'utilisateur (POST /users).

## 🔍 Améliorations Apportées

### 1. Logs Détaillés dans ApiUserService.cs

**Avant** :
```csharp
System.Diagnostics.Debug.WriteLine($"➕ Création utilisateur: {name} ({email})...");
var createdUser = await _apiClient.PostAsync<object, User>("/users", userData);
```

**Après** :
```csharp
System.Diagnostics.Debug.WriteLine($"➕ Création utilisateur: {name} ({email})...");
System.Diagnostics.Debug.WriteLine($"📤 Données envoyées:");
System.Diagnostics.Debug.WriteLine($"   - name: {name}");
System.Diagnostics.Debug.WriteLine($"   - email: {email}");
System.Diagnostics.Debug.WriteLine($"   - password_hash: {passwordHash}");
System.Diagnostics.Debug.WriteLine($"   - role: {role}");

var createdUser = await _apiClient.PostAsync<object, User>("users", userData);
```

### 2. Logs Détaillés dans ApiHttpClient.cs

**Ajout** :
```csharp
var json = JsonSerializer.Serialize(data, _jsonOptions);
System.Diagnostics.Debug.WriteLine($"📤 Request body JSON: {json}");

// ...après la requête...

System.Diagnostics.Debug.WriteLine($"📥 Response status: {response.StatusCode} ({(int)response.StatusCode})");
System.Diagnostics.Debug.WriteLine($"📥 Response body: {responseContent}");

if (!response.IsSuccessStatusCode)
{
    System.Diagnostics.Debug.WriteLine($"❌ Erreur HTTP: {response.StatusCode}");
    System.Diagnostics.Debug.WriteLine($"❌ Raison: {response.ReasonPhrase}");
    System.Diagnostics.Debug.WriteLine($"❌ Contenu de l'erreur: {responseContent}");
}
```

### 3. Correction des Endpoints (Sans Slash Initial)

Tous les endpoints ont été corrigés pour être cohérents avec le fix du slash final :

| Avant | Après | URL Finale |
|-------|-------|------------|
| `/users` | `users` | `https://reymond.alwaysdata.net/smartLockerApi/users` ✅ |
| `/users/{id}` | `users/{id}` | `https://reymond.alwaysdata.net/smartLockerApi/users/1` ✅ |

**Rappel** : Avec `BaseAddress` qui a un **slash final**, on ne met **pas de slash initial** dans les endpoints.

## 📊 Logs Attendus Maintenant

### Création de Compte - Logs Complets

```
📝 Début création de compte pour: test@example.com
📝 Création de compte pour: test@example.com
🔄 Token expiré ou inexistant, obtention d'un nouveau token...
🔐 Tentative login API: Smart
📤 POST URL complète: https://reymond.alwaysdata.net/smartLockerApi/login
📤 Request body: {"username":"Smart","password":"Locker"}
📥 Response status: OK (200)
✅ Token JWT obtenu avec succès
✅ Token (début): eyJ0eXAiOiJKV1QiLCJhbGc...

➕ Création utilisateur: Test User (test@example.com)...
📤 Données envoyées:
   - name: Test User
   - email: test@example.com
   - password_hash: password123
   - role: user
✅ Header Authorization configuré: Bearer eyJ0eXAiOiJKV1QiLCJ...
📤 POST users
📤 Request body JSON: {"name":"Test User","email":"test@example.com","password_hash":"password123","role":"user"}
📥 Response status: Created (201)
📥 Response body: {"id":15,"name":"Test User","email":"test@example.com","role":"user","created_at":"2024-01-15 10:30:00"}
✅ Utilisateur créé avec ID=15
✅ Utilisateur créé dans l'API avec ID=15
```

### Si Erreur - Logs Complets

```
➕ Création utilisateur: Test User (test@example.com)...
📤 Données envoyées:
   - name: Test User
   - email: test@example.com
   - password_hash: password123
   - role: user
📤 POST users
📤 Request body JSON: {"name":"Test User","email":"test@example.com","password_hash":"password123","role":"user"}
📥 Response status: BadRequest (400)
📥 Response body: {"error":"Email already exists"}
❌ Erreur HTTP: BadRequest
❌ Raison: Bad Request
❌ Contenu de l'erreur: {"error":"Email already exists"}
❌ Échec création utilisateur - Réponse null
```

## 🔍 Analyse des Erreurs Possibles

### Erreur 400 (Bad Request)
**Causes possibles** :
- Email déjà existant
- Format de données incorrect
- Champ obligatoire manquant

**À vérifier dans les logs** :
- Le body JSON envoyé
- Le message d'erreur dans la réponse

### Erreur 401 (Unauthorized)
**Cause** : Token JWT invalide ou expiré

**À vérifier dans les logs** :
- `✅ Header Authorization configuré: Bearer ...`
- Si le token est bien présent et valide

### Erreur 404 (Not Found)
**Cause** : Endpoint incorrect

**À vérifier dans les logs** :
- L'URL complète : doit être `https://reymond.alwaysdata.net/smartLockerApi/users`

### Erreur 500 (Internal Server Error)
**Cause** : Erreur côté serveur

**À vérifier dans les logs** :
- Le contenu de l'erreur dans la réponse

## 🧪 Test

**Relancez l'app et essayez de créer un compte.**

Consultez les logs pour voir :
1. ✅ Le body JSON exact qui est envoyé
2. ✅ Le status code de la réponse
3. ✅ Le contenu de la réponse (succès ou erreur)

## 📝 Fichiers Modifiés

1. ✅ `ApiUserService.cs` - Logs détaillés + endpoints sans slash initial
2. ✅ `ApiHttpClient.cs` - Affichage du body JSON et de la réponse complète

## 🎯 Prochaines Étapes

Avec ces nouveaux logs, on pourra identifier précisément :
- ✅ Si le body JSON est correct
- ✅ Si l'endpoint est appelé correctement
- ✅ Quel est le code d'erreur exact
- ✅ Quel est le message d'erreur de l'API

**Relancez l'app et partagez les nouveaux logs pour qu'on puisse identifier le problème exact !** 🚀
