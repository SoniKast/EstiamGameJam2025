# Configuration de l'interaction avec la bombe

## 🎯 Solution rapide avec les nouveaux scripts

### Option A : Configuration automatique (Recommandé)
1. **Créer un GameObject vide** dans la scène
2. **Ajouter le script `TagAndLayerSetup`**
3. **Clic droit sur le component → "Setup Tags and Layers"**
4. Tous les tags et layers seront créés automatiquement !

### Option B : Sur la bombe directement
1. **Sélectionner la bombe**
2. **Ajouter le script `BombSetup`**
3. La bombe sera configurée automatiquement avec :
   - Tag "Interactable"
   - Layer "Interactable"
   - Collider2D en trigger
   - Rigidbody2D kinematic
   - InteractableObject

## Tags requis dans Unity

### 1. **Sur le Player** :
- Tag : **"Player"**
- Layer : Default

### 2. **Sur la Bombe** :
- Tag : **"Interactable"**
- Layer : **"Interactable"** (layer 6)

### 3. **Autres objets enregistrables** :
- Tag : **"Recordable"** (pour le système de rewind)

## Configuration de la bombe dans Unity

### 1. **Sélectionner l'objet Bombe dans la scène**

### 2. **Ajouter les composants nécessaires** :
- **Collider2D** (BoxCollider2D ou CircleCollider2D)
  - ✅ Is Trigger = true
- **InteractableObject** (script)
  - Interaction Prompt : "Appuyez sur E pour désamorcer"
  - Interaction Range : 2
  - Mini Game To Start : NumberSequence (ou autre)
  - ✅ Disable After Use
  
### 3. **Dans l'Inspector de InteractableObject** :
- **Events > On Interact** : 
  - Ajouter la référence à la bombe
  - Choisir Transform > localScale
  - Définir une valeur plus grande (ex: 1.5, 1.5, 1.5)

### 4. **Configuration du Player** :
- Vérifier que le PlayerController est attaché
- Vérifier qu'il a le tag "Player"
- S'assurer qu'il a un Collider2D

## Structure complète de la bombe :
```
Bomb GameObject
├── SpriteRenderer (avec le sprite de la bombe)
├── Animator (pour les animations)
├── BoxCollider2D (Is Trigger = true)
├── InteractableObject (script)
└── (Optionnel) AudioSource pour les sons
```

## Ordre de configuration dans Unity :

1. **Tags** (Edit > Project Settings > Tags and Layers) :
   - Ajouter "Interactable" dans Tags
   - Ajouter "Interactable" dans Layer 6

2. **Sur la bombe** :
   - Tag = "Interactable"
   - Layer = "Interactable"
   - Ajouter InteractableObject
   - Configurer le collider en trigger

3. **Sur le player** :
   - Tag = "Player"
   - Vérifier PlayerController

## Test rapide :
1. Approche-toi de la bombe
2. Un message devrait apparaître
3. Appuie sur E
4. La bombe devrait s'agrandir ou déclencher un mini-jeu

## Problèmes courants :
- **Pas de message** : Vérifier le collider trigger sur la bombe
- **E ne marche pas** : Vérifier que le tag est "Interactable"
- **Erreur de layer** : Créer le layer "Interactable" dans les settings