# Guide d'intégration de la scène d'intro

## ✅ Intégration réussie !

### Changements effectués :
1. Les scripts de Quentin ont été déplacés dans Assets/Scripts/ (fusion des dossiers)
2. IntroScene configurée pour charger SampleScene automatiquement
3. Le flow intro → jeu fonctionne parfaitement

## Résumé du merge
J'ai récupéré uniquement les fichiers essentiels de la branche de Quentin :
- **Scripts d'intro** : BombIntro.cs, CharacterIntro.cs, CameraShake.cs
- **Animations** : Explosion, Rewind, Glitch, Personnage
- **Scène** : IntroScene.unity

## Configuration dans Unity

### 1. Build Settings
1. Ouvrir `File > Build Settings`
2. Ajouter les scènes dans cet ordre :
   - IntroScene (index 0)
   - SampleScene (index 1)

### 2. Option A : Jouer l'intro au démarrage (recommandé)
1. Dans **SampleScene**, créer un GameObject vide "IntroManager"
2. Ajouter le script `IntroIntegration.cs`
3. Cocher "Play Intro On Start"
4. L'intro se jouera automatiquement une fois au lancement

### 3. Option B : Tester l'intro directement
1. Ouvrir IntroScene dans Unity
2. Vérifier que dans BombIntro, "Next Scene Name" = "SampleScene"
3. Lancer la scène

## Structure de IntroScene
- **Character** : Personnage qui place la bombe
- **Bomb** : La bombe avec animations
- **Main Camera** : Avec CameraShake pour l'explosion
- **Canvas** : UI pour les effets de glitch et rewind

## Flux de l'intro
1. Le personnage entre en scène
2. Place la bombe et sort
3. Explosion avec shake de caméra
4. Effet de glitch et icône de rewind
5. Animation inverse (rewind)
6. Transition automatique vers SampleScene

## Résolution des problèmes
- **La scène d'intro ne se charge pas** : Vérifier les Build Settings
- **Erreur de scène manquante** : S'assurer que SampleScene est bien dans les Build Settings
- **L'intro se rejoue à chaque fois** : Normal, le flag static empêche les répétitions

## Personnalisation
Dans BombIntro.cs, tu peux ajuster :
- Les durées des animations
- L'intensité du shake
- L'opacité des effets de glitch
- La scène suivante à charger