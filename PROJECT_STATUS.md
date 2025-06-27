# État du Projet - EstiamGameJam2025
Date : 26 juin 2025

## 🎮 Vue d'ensemble
- **Thème** : 60 secondes
- **Type** : Jeu 2D Unity avec time-rewind
- **État** : Phase d'intégration

## ✅ Ce qui est fait

### 1. Scène de jeu
- ✅ Intégration de l'image de fond (Home.png) de Balla
- ✅ Player fonctionnel avec mouvement et animations
- ✅ Système de collision complet (murs mappés)
- ✅ Objet interactif : Bombe placée dans la scène

### 2. Scripts principaux
- ✅ **PlayerController** : Mouvement fluide, système de rewind intégré
- ✅ **CollisionMapper** : Outil pour créer les collisions sur l'image de fond
- ✅ **InteractableObject** : Objets avec lesquels interagir (touche E)
- ✅ **3 Mini-jeux** adaptés du collègue :
  - SwitchMiniGame (interrupteurs)
  - CableMatchMiniGame (connexion câbles)
  - NumberSequenceMiniGame (séquence 0-9)

### 3. Configuration actuelle
- Player configuré avec :
  - Tag : "Player"
  - CircleCollider2D (radius: 0.1)
  - Rigidbody2D (Gravity: 0, Collision: Continuous)
  - Scripts : PlayerController, PlayerSetup, PlayerAnimationController

- Bombe interactive :
  - Position : (-6.03, 6.93)
  - Scale : 0.3
  - Effet : Grandit quand on approche (pulse)
  - Interaction : Touche E

- Collisions :
  - Tous les murs de la maison mappés
  - 17 zones de collision créées
  - Layer par défaut (Wall layer optionnel)

## 📋 Prochaines étapes

### 1. Canvas et UI (Priorité HAUTE)
- [ ] Créer le Canvas principal
- [ ] UI pour les mini-jeux
- [ ] Prompt d'interaction "[E] Interagir"
- [ ] Timer 60 secondes

### 2. Séquence de catastrophe (Priorité HAUTE)
- [ ] Phase 1 : Exploration libre (5 secondes)
- [ ] Phase 2 : Déclenchement catastrophe (explosion)
- [ ] Phase 3 : Animation rewind (3 secondes)
- [ ] Phase 4 : Investigation avec timer 60s

### 3. GameManager principal
- [ ] Machine à états complète
- [ ] Coordination des phases de jeu
- [ ] Gestion victoire/défaite

### 4. Intégration mini-jeux
- [ ] Lier la bombe au mini-jeu approprié
- [ ] Tester le flux complet

## 🎯 Objectif immédiat
Implémenter la séquence de catastrophe pour avoir un premier prototype jouable avec :
1. Le joueur explore
2. La bombe explose
3. Rewind temporel
4. 60 secondes pour désamorcer

## 📝 Notes techniques
- La scène utilise l'image Home.png comme fond
- Les collisions sont gérées par CollisionMapper
- Le système est prêt pour l'ajout du GameManager principal
- Tous les scripts de base sont fonctionnels

## 🐛 Problèmes connus
- Canvas manquant pour l'UI
- GameManager pas encore créé
- Séquence de jeu pas implémentée

## 👥 Collaboration
- **Rateb** : Scripts et GameManagers
- **Balla** : Interface et assets visuels
- **Autres** : Mini-jeux (adaptés)