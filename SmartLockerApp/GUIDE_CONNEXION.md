# 🚀 Guide Rapide - Connexion SmartLocker App

## ✅ Corrections Effectuées

### 1. URL API Corrigée (CRITIQUE)
- ✅ URL correcte avec **slash final** : `https://reymond.alwaysdata.net/smartLockerApi/`
- ⚠️ **IMPORTANT** : Le slash final est **obligatoire** pour que HttpClient combine correctement les URLs
- ✅ Credentials JWT : `username: "Smart"`, `password: "Locker"`

### 2. Authentification Améliorée
- ✅ Messages d'erreur plus clairs
- ✅ Logs détaillés pour debugging
- ✅ Recherche utilisateur dans la BDD via l'API

### 3. Création de Compte dans l'API
- ✅ Les nouveaux comptes sont maintenant créés dans la base de données
- ✅ Fallback local si l'API est indisponible
- ✅ SignupPage utilise maintenant IDataService au lieu de AppStateService

### 4. Fix URL 404 NotFound
- ✅ Ajout du slash final à l'URL de base pour éviter que HttpClient ne remplace `/smartLockerApi`
- ✅ URL complète affichée dans les logs pour debugging

## 📱 Comment Utiliser l'Application

### Première Connexion (Nouveau Compte)

1. **Ouvrir l'app** → Page de connexion
2. **Cliquer sur** "Créer un compte" ou "Pas encore de compte ?"
3. **Remplir le formulaire** :
   - Prénom : `Nolan`
   - Nom : `REYMOND`
   - Email : `nolan@example.com`
   - Mot de passe : `votre_mot_de_passe`
4. **Cliquer sur** "Créer mon compte"
5. **Résultat** : ✅ Compte créé dans la BDD et connexion automatique

### Connexions Suivantes

1. **Ouvrir l'app** → Page de connexion
2. **Entrer vos identifiants** :
   - Email : `nolan@example.com`
   - Mot de passe : `votre_mot_de_passe`
3. **Cliquer sur** "Se connecter"
4. **Résultat** : ✅ Connexion réussie

## 🔍 Messages et Leur Signification

### Messages de Succès ✅

| Message | Signification |
|---------|---------------|
| **"Connexion réussie"** | Vous êtes connecté ! L'utilisateur existe dans la BDD |
| **"Compte créé avec succès !"** | Votre compte a été créé dans la BDD et vous êtes connecté |

### Messages d'Erreur ❌

| Message | Cause | Solution |
|---------|-------|----------|
| **"Email ou mot de passe incorrect. Veuillez créer un compte..."** | L'utilisateur n'existe pas dans la BDD | Créer un compte via "Créer un compte" |
| **"Erreur de connexion à l'API. Vérifiez votre connexion Internet."** | Pas de connexion à l'API | Vérifier votre connexion Internet |
| **"Compte créé localement (API indisponible)"** | L'API n'est pas disponible mais le compte est créé localement | Réessayer plus tard pour synchroniser avec la BDD |

## 🐛 Debug : Logs à Consulter

Si vous avez un problème, consultez les logs dans Visual Studio (Debug Output) :

### Connexion Réussie - Logs Attendus :
```
🔐 Tentative de connexion pour: nolan@example.com
🔄 Token expiré ou inexistant, obtention d'un nouveau token...
🔐 Tentative login API: Smart
📤 POST URL: https://reymond.alwaysdata.net/smartLockerApi/login
📤 Request body: {"username":"Smart","password":"Locker"}
📥 Response status: OK
✅ Token JWT obtenu avec succès
✅ Token JWT obtenu
🔍 Recherche utilisateur par email: nolan@example.com...
📋 Récupération de tous les utilisateurs...
📤 GET /users
📥 Response: OK
✅ 5 utilisateurs récupérés
✅ Utilisateur trouvé: Nolan REYMOND
✅ Utilisateur trouvé: Nolan REYMOND (ID: 1)
```

### Création de Compte - Logs Attendus :
```
📝 Création de compte pour: nolan@example.com
🔄 Token expiré ou inexistant, obtention d'un nouveau token...
🔐 Tentative login API: Smart
📤 POST URL: https://reymond.alwaysdata.net/smartLockerApi/login
📥 Response status: OK
✅ Token JWT obtenu avec succès
➕ Création utilisateur: Nolan REYMOND (nolan@example.com)...
📤 POST /users
📥 Response: OK
✅ Utilisateur créé avec ID=10
✅ Utilisateur créé dans l'API avec ID=10
```

### Erreur de Connexion - Logs Attendus :
```
🔐 Tentative de connexion pour: inconnu@example.com
✅ Token JWT obtenu
🔍 Recherche utilisateur par email: inconnu@example.com...
📋 Récupération de tous les utilisateurs...
⚠️ Utilisateur non trouvé
❌ Aucun utilisateur trouvé avec l'email: inconnu@example.com
```

## 🎯 Checklist de Vérification

Avant de tester, assurez-vous que :

- ✅ L'URL de l'API est `https://reymond.alwaysdata.net/smartLockerApi/` **avec le slash final** (CRITIQUE !)
- ✅ Les credentials JWT sont `Smart` / `Locker`
- ✅ Vous avez une connexion Internet
- ✅ Vous avez rebuild le projet après les modifications
- ✅ Vous utilisez d'abord "Créer un compte" si c'est votre première utilisation

### ⚠️ Pourquoi le Slash Final est Important ?

Sans le slash final, HttpClient combine mal les URLs :
- ❌ `https://reymond.alwaysdata.net/smartLockerApi` + `/login` = `https://reymond.alwaysdata.net/login` (MAUVAIS)
- ✅ `https://reymond.alwaysdata.net/smartLockerApi/` + `login` = `https://reymond.alwaysdata.net/smartLockerApi/login` (BON)

## 🔧 En Cas de Problème

### Problème : "Erreur d'authentification avec l'API"

**Cause possible** : Pas de token JWT

**Solution** :
1. Vérifier votre connexion Internet
2. Vérifier que l'API est accessible : `https://reymond.alwaysdata.net/smartLockerApi/`
3. Consulter les logs pour voir l'erreur exacte

### Problème : "Email ou mot de passe incorrect"

**Cause** : L'utilisateur n'existe pas dans la BDD

**Solution** :
1. Créer un compte via "Créer un compte"
2. OU vérifier que vous utilisez le bon email

### Problème : L'API répond 404

**Vérification** :
- URL doit être `smartLockerApi` (avec A majuscule)
- Endpoint `/login` doit avoir le slash

## 📞 Support

Si les problèmes persistent après avoir suivi ce guide :
1. Consultez les logs détaillés dans Debug Output
2. Vérifiez que l'API est accessible dans un navigateur
3. Vérifiez les credentials JWT (`Smart` / `Locker`)

---

**🎉 Tout est maintenant configuré pour fonctionner correctement avec l'API SmartLocker en production !**
