# Gestion d'une Location de Voitures — EF Core & SQLite

Application desktop **C# (.NET 10 Windows Forms)** de gestion d'une agence de location de voitures utilisant **Entity Framework Core (Code First)** et **SQLite**.

## Binôme
* **Yassine Latrous**
* **Rayen Chafei**

## Architecture & Fonctionnalités
* **Modèle Objet (`Models/`) :** Hiérarchie `Personne` (`Client`, `Agent`), `Vehicule`, `Entretien`, `Reservation` et `ContratLocation` avec énumérations métiers (`Enums.cs`).
* **Persistance EF Core & SQLite (`Data/AppDbContext.cs`) :**
  * Mapping Code First complet (`DbSet<T>`, relations One-to-Many et One-to-One, suppression en cascade sur l'historique d'entretien).
  * Chemin de base de données portable (`GetDatabasePath()`) résolu dynamiquement depuis le dossier du `.csproj`, garantissant la compatibilité immédiate après clonage sur n'importe quel poste.
  * Exécution automatique de `db.Database.Migrate()` au démarrage.
* **Interface Graphique CRUD (`Forms/MainForm.cs`) :**
  * **Gestion des Clients :** Ajout, modification, suppression, recherche instantanée insensible à la casse, contrôle des doublons et verrouillage contextuel des boutons.
  * **Gestion des Véhicules :** Suivi du parc automobile (immatriculation, kilométrage, tarif journalier, statut `Disponible` / `Loue` / `Maintenance`) avec recherche et contrôle d'unicité.
  * **Gestion des Réservations :** Création et clôture de locations avec calcul automatique du montant et mise à jour synchronisée du statut des véhicules.

## Lancement du Projet
1. Cloner le dépôt et ouvrir `GestionLocationVoitures.sln` dans **Visual Studio 2022 / 2026**.
2. Appuyer sur **`F5`** pour compiler et lancer l'application (la base `location_voitures.db` est incluse et reconstruite automatiquement via les migrations si absente).