# 🚀 Guide Rapide - Ajouter une bombe interactive

## En 5 étapes simples :

### 1️⃣ **Créer le tag (une seule fois)**
```
Edit → Project Settings → Tags and Layers
Tags → + → "Interactable"
```

### 2️⃣ **Placer la bombe**
- Glisser le sprite de bombe dans la scène
- Position où tu veux

### 3️⃣ **Configurer la bombe**
Add Component :
- `SimpleBombInteraction`
- `Box Collider 2D` (ajouté automatiquement)

### 4️⃣ **Assigner le mini-jeu**
Dans SimpleBombInteraction :
- **Bomb Prefab To Activate** : Glisser Lvl1 depuis Assets/Bombe
- Cocher les options voulues

### 5️⃣ **Tester**
- Play
- Approcher avec le joueur
- Appuyer sur E

## 🐛 Problèmes courants

| Problème | Solution |
|----------|----------|
| "Touche E ne marche pas" | Vérifier tag "Player" sur le joueur |
| "Prefab ne s'active pas" | Placer d'abord Lvl1 dans la scène |
| "Joueur bouge encore" | Cocher "Pause Player During Minigame" |
| "Peut interagir plusieurs fois" | Cocher "Disable After Use" |

## 💡 Tips

- **Debug** : Ajouter temporairement `InteractionDebugger`
- **Visuel** : La bombe devient jaune = joueur proche
- **Distance** : Ajuster dans Inspector si trop loin/proche

---
*Copier une bombe existante est plus rapide que tout reconfigurer !*