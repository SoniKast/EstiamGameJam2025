using UnityEngine;

namespace EstiamGameJam2025
{
    public class Level1CollisionPreset : MonoBehaviour
    {
        [Header("Configuration Niveau 1")]
        [SerializeField] private CollisionMapper collisionMapper;
        
        void Start()
        {
            if (collisionMapper == null)
            {
                collisionMapper = GetComponent<CollisionMapper>();
                if (collisionMapper == null)
                {
                    collisionMapper = gameObject.AddComponent<CollisionMapper>();
                }
            }
            
            SetupLevel1Collisions();
        }
        
        private void SetupLevel1Collisions()
        {
            // Configuration spécifique pour votre image Home.png
            // Ces valeurs sont à ajuster selon la disposition réelle de votre niveau
            
            // Murs extérieurs (estimation basée sur une image de maison typique)
            collisionMapper.AddCollisionArea(new Vector2(0, 5.5f), new Vector2(12, 0.5f), "Mur_Haut");
            collisionMapper.AddCollisionArea(new Vector2(0, -5.5f), new Vector2(12, 0.5f), "Mur_Bas");
            collisionMapper.AddCollisionArea(new Vector2(-6, 0), new Vector2(0.5f, 11), "Mur_Gauche");
            collisionMapper.AddCollisionArea(new Vector2(6, 0), new Vector2(0.5f, 11), "Mur_Droit");
            
            // Murs intérieurs (exemples - à adapter)
            // Mur horizontal du milieu
            collisionMapper.AddCollisionArea(new Vector2(-2, 0), new Vector2(4, 0.3f), "Mur_Interieur_H1");
            collisionMapper.AddCollisionArea(new Vector2(2, 0), new Vector2(4, 0.3f), "Mur_Interieur_H2");
            
            // Mur vertical séparateur
            collisionMapper.AddCollisionArea(new Vector2(0, 2), new Vector2(0.3f, 4), "Mur_Interieur_V1");
            
            // Objets/Meubles (exemples)
            // Table
            collisionMapper.AddCollisionArea(new Vector2(-3, 2), new Vector2(1.5f, 1f), "Table");
            
            // Canapé
            collisionMapper.AddCollisionArea(new Vector2(3, -2), new Vector2(2f, 1f), "Canape");
            
            // Portes (sans collision pour permettre le passage)
            // Les portes sont gérées différemment, peut-être avec des triggers
            
            Debug.Log("Collisions du niveau 1 configurées!");
        }
        
        // Méthode pour ajuster les collisions en temps réel
        [ContextMenu("Recharger les Collisions")]
        public void ReloadCollisions()
        {
            // Nettoyer les anciennes collisions
            foreach (Transform child in transform)
            {
                if (child.name.StartsWith("Collider_"))
                {
                    if (Application.isPlaying)
                        Destroy(child.gameObject);
                    else
                        DestroyImmediate(child.gameObject);
                }
            }
            
            // Recréer les collisions
            SetupLevel1Collisions();
        }
    }
}