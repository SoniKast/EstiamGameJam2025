using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EstiamGameJam2025
{
    /// <summary>
    /// Script d'aide pour configurer les GIF en sprites dans Unity
    /// </summary>
    public class GIFtoSpriteHelper : MonoBehaviour
    {
        [Header("Instructions pour utiliser les GIF")]
        [TextArea(10, 20)]
        [SerializeField] private string instructions = @"
COMMENT UTILISER LES GIF DANS UNITY:

1. IMPORTER LES GIF:
   - Unity ne supporte pas directement les GIF animés
   - Les GIF sont importés comme des textures simples
   
2. CONVERTIR EN SPRITE SHEET:
   Option A - Utiliser un outil externe:
   - Utiliser un convertisseur GIF to Sprite Sheet en ligne
   - Ou utiliser des outils comme Aseprite, Photoshop
   
   Option B - Utiliser le système manuel:
   - Extraire chaque frame du GIF
   - Les importer séparément dans Unity
   
3. CONFIGURER LES SPRITES:
   - Sélectionner la texture importée
   - Sprite Mode: Multiple
   - Pixels Per Unit: 100
   - Filter Mode: Point (no filter) pour le pixel art
   - Compression: None
   - Cliquer sur 'Sprite Editor'
   - Découper les frames (Slice > Grid By Cell Size)
   
4. ASSIGNER AU PLAYERANIMATIONCONTROLLER:
   - Sur le GameObject Player
   - Ajouter le component PlayerAnimationController
   - Glisser les sprites dans les arrays correspondants:
     * IdleF.gif → Idle Down Sprites
     * IdleD.gif → Idle Down Sprites (alternative)
     * IdleL.gif → Idle Left Sprites
     * IdleR.gif → Idle Right Sprites
     * WalkF.gif → Walk Up Sprites
     * WalkD.gif → Walk Down Sprites
     * WalkL.gif → Walk Left Sprites
     * WalkR.gif → Walk Right Sprites
   
5. CONFIGURATION RECOMMANDÉE:
   - Animation Speed: 0.1 (ajuster selon vos GIF)
   - Assurez-vous que le Player a aussi:
     * SpriteRenderer
     * PlayerController
     * Rigidbody2D
     * Collider2D
";

#if UNITY_EDITOR
        [Space]
        [Header("Configuration Automatique")]
        [SerializeField] private GameObject playerPrefab;
        
        [ContextMenu("Setup Player avec Animation Controller")]
        void SetupPlayer()
        {
            if (playerPrefab == null)
            {
                Debug.LogError("Veuillez assigner le prefab du joueur!");
                return;
            }
            
            // Ajouter les components nécessaires
            if (!playerPrefab.GetComponent<SpriteRenderer>())
                playerPrefab.AddComponent<SpriteRenderer>();
                
            if (!playerPrefab.GetComponent<PlayerController>())
                playerPrefab.AddComponent<PlayerController>();
                
            if (!playerPrefab.GetComponent<PlayerAnimationController>())
                playerPrefab.AddComponent<PlayerAnimationController>();
                
            if (!playerPrefab.GetComponent<Rigidbody2D>())
            {
                var rb = playerPrefab.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0; // Pour un jeu top-down
            }
            
            if (!playerPrefab.GetComponent<BoxCollider2D>())
                playerPrefab.AddComponent<BoxCollider2D>();
            
            // Tag le joueur
            playerPrefab.tag = "Player";
            
            Debug.Log("Player configuré avec succès! N'oubliez pas d'assigner les sprites.");
        }
        
        [ContextMenu("Créer Animation depuis GIF (Guide)")]
        void CreateAnimationGuide()
        {
            Debug.Log(@"
GUIDE POUR CRÉER DES ANIMATIONS DEPUIS LES GIF:

1. Sélectionnez votre GIF dans le Project
2. Dans l'Inspector:
   - Texture Type: Sprite (2D and UI)
   - Sprite Mode: Multiple
   - Cliquez 'Apply'
3. Cliquez sur 'Sprite Editor'
4. Dans le Sprite Editor:
   - Slice > Type: Grid By Cell Size
   - Pixel Size: (largeur d'une frame, hauteur d'une frame)
   - Cliquez 'Slice' puis 'Apply'
5. Les sprites découpés apparaîtront sous le GIF
6. Sélectionnez tous les sprites d'une animation
7. Glissez-les dans le bon slot du PlayerAnimationController
");
        }
#endif
    }
}