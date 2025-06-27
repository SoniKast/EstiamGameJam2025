using UnityEngine;

namespace EstiamGameJam2025
{
    /// <summary>
    /// Script simple pour configurer la bombe automatiquement
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class SimpleTagSetup : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private bool setupOnStart = true;
        
        void Start()
        {
            if (setupOnStart)
            {
                SetupBomb();
            }
        }
        
        [ContextMenu("Setup Bomb Now")]
        public void SetupBomb()
        {
            Debug.Log("[SimpleTagSetup] Configuration de la bombe...");
            
            // 1. Tag
            gameObject.tag = "Interactable";
            Debug.Log("✓ Tag configuré : Interactable");
            
            // 2. Collider en trigger
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                col.isTrigger = true;
                Debug.Log("✓ Collider configuré en trigger");
            }
            
            // 3. Ajouter Rigidbody2D si absent
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
                Debug.Log("✓ Rigidbody2D ajouté");
            }
            rb.bodyType = RigidbodyType2D.Kinematic;
            
            // 4. Ajouter InteractableObject si absent
            InteractableObject interactable = GetComponent<InteractableObject>();
            if (interactable == null)
            {
                interactable = gameObject.AddComponent<InteractableObject>();
                Debug.Log("✓ InteractableObject ajouté");
            }
            
            Debug.Log("[SimpleTagSetup] Configuration terminée !");
            Debug.Log("N'oublie pas de créer le tag 'Interactable' dans Project Settings > Tags and Layers");
        }
        
        [ContextMenu("Remove This Script")]
        void RemoveScript()
        {
            Debug.Log("Script supprimé après configuration");
            DestroyImmediate(this);
        }
    }
}