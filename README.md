# Unity Chase Game 🎮

## 🚧 Note de développement

**Fonctionnalités prévues mais non implémentées :**

J'aurais voulu ajouter plusieurs fonctionnalités supplémentaires à ce projet : 
- Un système de collecte d'objets pour se défendre contre les poursuivants
- Des power-ups et boosts pour améliorer les capacités du joueur

Malheureusement, mon ordinateur ne supportait pas très bien Unity lors du développement. Le projet devenait de plus en plus lourd, ce qui causait : 
- Des plantages fréquents lors de l'édition
- Des temps de chargement très longs
- Des difficultés à continuer le développement

Malgré ces limitations techniques, j'ai réussi à créer une base fonctionnelle du jeu. 

---

## 📖 Description

Unity Chase Game est un jeu de poursuite développé avec Unity.  Le joueur doit échapper à des poursuivants dans un environnement urbain en low poly. 

## 🎯 Caractéristiques actuelles

- Environnement urbain 3D avec assets low poly (SimplePoly City)
- Système de contrôle du personnage
- Animations de personnage (Adventure Character)
- Interface utilisateur avec TextMesh Pro
- Système d'input moderne (Unity Input System)

## 🛠️ Technologies utilisées

- **Moteur** : Unity
- **Langage** : C#
- **Assets** : 
  - SimplePoly City - Low Poly Assets
  - Adventure Character
  - Sketchfab For Unity
- **Packages** :
  - Unity Input System
  - TextMesh Pro

## 📁 Structure du projet

```
Unity_ChaseGame/
├── Assets/
│   ├── Adventure_Character/      # Modèles et animations du personnage
│   ├── Models/                    # Modèles 3D personnalisés
│   ├── Scenes/                    # Scènes du jeu
│   ├── Scripts/                   # Scripts C# du gameplay
│   ├── Settings/                  # Paramètres du projet
│   ├── SimplePoly City - Low Poly Assets/  # Assets de la ville
│   ├── Sketchfab For Unity/      # Intégration Sketchfab
│   ├── TextMesh Pro/             # Assets TextMesh Pro
│   └── InputSystem_Actions. inputactions  # Configuration des contrôles
├── Packages/                      # Packages Unity
└── ProjectSettings/              # Paramètres du projet Unity
```

## 🎮 Contrôles

Le jeu utilise le nouveau système Input System de Unity pour une gestion moderne des contrôles. 

## 🚀 Installation

1. Clonez ce repository : 
```bash
git clone https://github.com/GilianC/Unity_ChaseGame.git
```

2. Ouvrez le projet avec Unity (version compatible avec les packages utilisés)

3. Ouvrez la scène principale dans `Assets/Scenes/`

4. Appuyez sur Play pour tester le jeu

## ⚙️ Configuration requise

### Développement
- Unity Editor (version recommandée selon les ProjectSettings)
- Visual Studio ou IDE compatible
- Configuration minimale : voir les limitations mentionnées ci-dessus

### Exécution
- À définir selon les builds finales

## 📝 Licence

Ce projet utilise des assets tiers qui peuvent avoir leurs propres licences : 
- SimplePoly City - Low Poly Assets
- Adventure Character
- Sketchfab For Unity

Vérifiez les licences individuelles de chaque asset avant toute utilisation commerciale.

## 👤 Auteur

**GilianC**
- GitHub: [@GilianC](https://github.com/GilianC)

## 🙏 Remerciements

- Créateurs des assets SimplePoly City
- Créateurs des modèles Adventure Character
- Communauté Unity

---

*Projet développé dans le cadre d'un apprentissage de Unity, avec les contraintes techniques mentionnées ci-dessus.*
