# 📱 SmartLocker – Application Mobile (MAUI)

## 🔎 Présentation

Cette application mobile fait partie du projet **SmartLocker**, un système de casiers connectés inspiré du concept des Amazon Lockers.  
Elle constitue l’interface principale entre **l’utilisateur** et le système global.  

Grâce à cette application, un utilisateur peut :  
- 🔐 Se connecter de manière sécurisée.  
- 📦 Réserver et verrouiller un casier via RFID ou empreinte digitale (sur place).  
- 📱 Déverrouiller un casier à distance depuis l’application.  
- ⏳ Suivre en temps réel l’utilisation de son casier (session active, durée restante).  
- 💳 Procéder à un paiement fictif à la fin de l’utilisation.  
- 📜 Consulter son historique de sessions.  

L’application est développée en **.NET MAUI (MVVM)** pour être **multiplateforme** (Android, iOS, Windows).  

---

## 🏗️ Architecture (Mobile)

📌 *Un schéma ou diagramme (UML ou synoptique mobile) peut être inséré ici.*  
*(Exemple : navigation entre pages, structure MVVM, appels API → Services → ViewModels → Views).*  

---

## ⚙️ Technologies Utilisées

- **Framework mobile** : .NET MAUI (.NET 8, MVVM)  
- **Pattern** : MVVM avec CommunityToolkit.Mvvm  
- **Langage** : C#  
- **Stockage sécurisé** : SecureStorage (JWT Token)  
- **API consommée** : [SmartLockerAPI](LIEN_VERS_REPO_API) (PHP Slim + JWT)  
- **Base de données** : MySQL (via API)  
- **Design** : Figma, V0.dev (génération IA, design system)  

---

## 📌 Fonctionnalités principales

- 👤 **Authentification** : connexion par email/mot de passe, gestion de session utilisateur.  
- 🔐 **Verrouillage/Déverrouillage** :  
  - via application mobile (commandes API),  
  - via badge RFID ou empreinte digitale sur place.  
- 📦 **Réservation d’un casier** avec définition d’une durée d’utilisation.  
- ⏳ **Session active** : affichage en temps réel de la durée restante, notifications avant expiration.  
- 💳 **Paiement fictif** : calcul du prix en fonction du temps d’utilisation.  
- 📜 **Historique des sessions** : liste des utilisations passées.  

---

## 📂 Structure du Code (MVVM)

```plaintext
SmartLockerAppMobile/
├── App.xaml / AppShell.xaml        # Navigation et ressources globales
├── Models/                         # DTOs (User, Locker, Session…)
├── Services/                       # Services API (AuthService, LockerService…)
├── ViewModels/                     # Logique des pages (LoginViewModel, HomeViewModel…)
├── Pages/                          # Interfaces utilisateurs (XAML)
└── Resources/                      # Styles, images, thèmes
