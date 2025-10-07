# 🔧 Debug - Problème Token JWT (NotFound)

## ❌ Problème Observé

Les logs montrent que l'obtention du token JWT échoue avec une erreur **404 NotFound** :

```
POST URL: https://reymond.alwaysdata.net/smartLockerApi/login
Request body: {"username":"Smart","password":"Locker"}
Login échoué: NotFound
❌ Impossible d'obtenir le token JWT, création locale uniquement
```

## 🔍 Causes Possibles

### 1. **Endpoint Incorrect**
L'endpoint `/login` pourrait ne pas exister ou avoir un nom différent.

**Variations possibles** :
- `/login` (avec slash)
- `login` (sans slash)
- `/auth/login`
- `/api/login`
- `/authenticate`

### 2. **URL de Base Incorrecte**
L'URL `https://reymond.alwaysdata.net/smartLockerApi` pourrait ne pas être correcte.

**Variations possibles** :
- `https://reymond.alwaysdata.net/smartLockerApi/` (avec slash final)
- `https://reymond.alwaysdata.net/smartlockerapi` (minuscules)
- `https://reymond.alwaysdata.net/api/smartlocker`

### 3. **Méthode HTTP Incorrecte**
Peut-être que l'API attend un GET au lieu d'un POST ?

### 4. **Format de Requête Incorrect**
Peut-être que l'API attend un format différent :
- Form data au lieu de JSON
- Noms de champs différents (`user` au lieu de `username`)

## ✅ Corrections Appliquées

### 1. Logs Détaillés Améliorés

**Dans `ApiAuthService.cs`** :
```csharp
// Affiche l'URL complète tentée
System.Diagnostics.Debug.WriteLine($"❌ URL complète: {_httpClient.BaseAddress}login");

// Teste automatiquement une variation sans le slash
var response2 = await _httpClient.PostAsync("login", content);
System.Diagnostics.Debug.WriteLine($"📥 Tentative 2 - Status: {response2.StatusCode}");
```

**Dans `ApiHttpClient.cs`** :
```csharp
// Affiche le début du token dans le header
System.Diagnostics.Debug.WriteLine($"✅ Header Authorization configuré: Bearer {token.Substring(0, 20)}...");
```

### 2. Test Automatique de Variations

Le code teste maintenant automatiquement deux variations :
1. `/login` (avec slash au début)
2. `login` (sans slash au début)

Si la première échoue, il essaie automatiquement la seconde.

## 🧪 Tests à Effectuer

### Test 1 : Vérifier l'Endpoint avec cURL

Ouvrez un terminal PowerShell et testez :

```powershell
# Test 1 : Avec /login
curl -X POST https://reymond.alwaysdata.net/smartLockerApi/login `
  -H "Content-Type: application/json" `
  -d '{"username":"Smart","password":"Locker"}'

# Test 2 : Sans le slash
curl -X POST https://reymond.alwaysdata.net/smartLockerApilogin `
  -H "Content-Type: application/json" `
  -d '{"username":"Smart","password":"Locker"}'

# Test 3 : Avec authentification de base
curl -X POST https://reymond.alwaysdata.net/smartLockerApi/login `
  -u "Smart:Locker"

# Test 4 : Variation de l'endpoint
curl -X POST https://reymond.alwaysdata.net/smartLockerApi/auth/login `
  -H "Content-Type: application/json" `
  -d '{"username":"Smart","password":"Locker"}'
```

### Test 2 : Vérifier avec Postman/Insomnia

1. **Créer une requête POST**
2. **URL** : `https://reymond.alwaysdata.net/smartLockerApi/login`
3. **Headers** :
   ```
   Content-Type: application/json
   ```
4. **Body (JSON)** :
   ```json
   {
     "username": "Smart",
     "password": "Locker"
   }
   ```
5. **Envoyer et observer la réponse**

### Test 3 : Vérifier dans un Navigateur

Ouvrez : `https://reymond.alwaysdata.net/smartLockerApi/`

Cela devrait vous montrer :
- Une page de documentation
- Une liste des endpoints disponibles
- Ou un message d'erreur qui confirme que l'API existe

## 📊 Analyse des Logs Futurs

Après avoir relancé l'app, cherchez dans les logs :

### Si le Token Fonctionne ✅
```
🔐 Tentative login API: Smart
📤 POST URL: https://reymond.alwaysdata.net/smartLockerApi/login
📥 Response status: OK
✅ Token JWT obtenu avec succès
✅ Token (début): eyJhbGciOiJIUzI1NiIsInR5cCI6...
```

### Si la Tentative 2 Fonctionne ✅
```
❌ Login échoué: NotFound
❌ Tentative avec d'autres variations...
📥 Tentative 2 - Status: OK
✅ Token obtenu avec succès (tentative 2)
```

### Si Toutes les Tentatives Échouent ❌
```
❌ Login échoué: NotFound
📥 Tentative 2 - Status: NotFound
❌ Pas de token dans la réponse
```

→ **Dans ce cas, l'endpoint `/login` n'existe probablement pas sur cette API**

## 🔧 Solutions Alternatives

### Solution 1 : Endpoint Différent

Si l'endpoint est différent, modifiez dans `ApiAuthService.cs` :

```csharp
// Essayez différentes variations
var response = await _httpClient.PostAsync("/auth/login", content);
// ou
var response = await _httpClient.PostAsync("/authenticate", content);
// ou
var response = await _httpClient.PostAsync("/api/login", content);
```

### Solution 2 : Méthode GET

Si l'API utilise GET avec Basic Auth :

```csharp
var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
var response = await _httpClient.GetAsync("/login");
```

### Solution 3 : Pas de Token

Si l'API n'utilise pas de JWT, mais Basic Auth directement :

```csharp
// Dans ApiHttpClient
private async Task<bool> ConfigureAuthHeaderAsync()
{
    var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("Smart:Locker"));
    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
    return true;
}
```

## 📞 Vérification avec le Créateur de l'API

**Questions à poser** :
1. Quel est l'endpoint exact pour obtenir le token JWT ?
2. Quelle méthode HTTP : GET ou POST ?
3. Quel format de body : JSON, Form Data, ou Basic Auth ?
4. Quels sont les noms de champs exacts : `username`/`password` ou `user`/`pass` ?
5. Y a-t-il une documentation Swagger/OpenAPI disponible ?

## 🎯 Actions Immédiates

1. **Relancer l'app** et consulter les nouveaux logs détaillés
2. **Tester l'endpoint avec cURL** comme montré ci-dessus
3. **Vérifier si la "Tentative 2" fonctionne** dans les logs
4. **Contacter le créateur de l'API** si nécessaire

---

**🔍 Les nouveaux logs vont nous aider à identifier précisément quel est le bon endpoint et format !**
