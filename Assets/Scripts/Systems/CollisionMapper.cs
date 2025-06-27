using System.Collections.Generic;
using UnityEngine;

namespace EstiamGameJam2025
{
    [System.Serializable]
    public class CollisionArea
    {
        public string name = "Wall";
        public Vector2 position;
        public Vector2 size = Vector2.one;
        public float rotation = 0f;
        public Color gizmoColor = Color.red;
    }

    public class CollisionMapper : MonoBehaviour
    {
        [Header("Collision Setup")]
        [SerializeField] private bool showGizmos = true;
        [SerializeField] private Color gizmoColor = new Color(1f, 0f, 0f, 0.3f);
        
        [Header("Collision Areas")]
        [SerializeField] private List<CollisionArea> collisionAreas = new List<CollisionArea>();
        
        [Header("Quick Setup")]
        [SerializeField] private bool usePresetForLevel1 = false;
        
        void Start()
        {
            if (usePresetForLevel1)
            {
                SetupLevel1Collisions();
            }
            
            CreateColliders();
        }
        
        private void SetupLevel1Collisions()
        {
            // Configuration prédéfinie pour le niveau 1
            // Ces valeurs devront être ajustées selon votre image
            collisionAreas.Clear();
            
            // Murs extérieurs (à ajuster selon votre image)
            collisionAreas.Add(new CollisionArea { 
                name = "Mur Haut", 
                position = new Vector2(0, 5), 
                size = new Vector2(20, 1) 
            });
            
            collisionAreas.Add(new CollisionArea { 
                name = "Mur Bas", 
                position = new Vector2(0, -5), 
                size = new Vector2(20, 1) 
            });
            
            collisionAreas.Add(new CollisionArea { 
                name = "Mur Gauche", 
                position = new Vector2(-10, 0), 
                size = new Vector2(1, 10) 
            });
            
            collisionAreas.Add(new CollisionArea { 
                name = "Mur Droit", 
                position = new Vector2(10, 0), 
                size = new Vector2(1, 10) 
            });
            
            // Ajoutez ici d'autres murs intérieurs selon votre niveau
        }
        
        private void CreateColliders()
        {
            // Supprimer les anciens colliders
            foreach (BoxCollider2D col in GetComponentsInChildren<BoxCollider2D>())
            {
                if (Application.isPlaying)
                    Destroy(col.gameObject);
                else
                    DestroyImmediate(col.gameObject);
            }
            
            // Créer les nouveaux colliders
            foreach (var area in collisionAreas)
            {
                GameObject colliderObj = new GameObject($"Collider_{area.name}");
                colliderObj.transform.SetParent(transform);
                colliderObj.transform.localPosition = area.position;
                colliderObj.transform.localRotation = Quaternion.Euler(0, 0, area.rotation);
                
                BoxCollider2D boxCollider = colliderObj.AddComponent<BoxCollider2D>();
                boxCollider.size = area.size;
                
                // Ajouter un layer pour les murs
                int wallLayer = LayerMask.NameToLayer("Wall");
                if (wallLayer != -1)
                {
                    colliderObj.layer = wallLayer;
                }
                else
                {
                    // Si le layer "Wall" n'existe pas, utiliser Default
                    colliderObj.layer = 0;
                    Debug.LogWarning("Layer 'Wall' n'existe pas. Utilisation du layer Default. Créez le layer 'Wall' dans Edit > Project Settings > Tags and Layers");
                }
            }
        }
        
        // Pour visualiser les colliders dans l'éditeur
        void OnDrawGizmos()
        {
            if (!showGizmos) return;
            
            foreach (var area in collisionAreas)
            {
                Gizmos.color = area.gizmoColor;
                
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(
                    transform.position + (Vector3)area.position, 
                    Quaternion.Euler(0, 0, area.rotation), 
                    Vector3.one
                );
                
                Gizmos.matrix = rotationMatrix;
                Gizmos.DrawCube(Vector3.zero, area.size);
                
                // Contour
                Gizmos.color = new Color(area.gizmoColor.r, area.gizmoColor.g, area.gizmoColor.b, 1f);
                Gizmos.DrawWireCube(Vector3.zero, area.size);
                
                Gizmos.matrix = Matrix4x4.identity;
            }
        }
        
        // Méthode helper pour ajouter un collider via code
        public void AddCollisionArea(Vector2 position, Vector2 size, string name = "Wall")
        {
            collisionAreas.Add(new CollisionArea 
            { 
                name = name, 
                position = position, 
                size = size 
            });
            
            if (Application.isPlaying)
            {
                CreateColliders();
            }
        }
    }
}