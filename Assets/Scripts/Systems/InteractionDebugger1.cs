using UnityEngine;

namespace EstiamGameJam2025
{
    /// <summary>
    /// Script de debug pour tester les interactions
    /// </summary>
    public class InteractionDebugger : MonoBehaviour
    {
        [Header("Debug Settings")]
        [SerializeField] private bool showDebugLogs = true;
        [SerializeField] private bool showVisualFeedback = true;
        [SerializeField] private KeyCode interactKey = KeyCode.E;
        
        [Header("Status")]
        [SerializeField] private bool playerInRange = false;
        [SerializeField] private GameObject currentPlayer = null;
        [SerializeField] private float distanceToPlayer = 999f;
        
        private SpriteRenderer spriteRenderer;
        private Color originalColor;
        
        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                originalColor = spriteRenderer.color;
            }
            
            // Vérifier la configuration
            Debug.Log($"[InteractionDebugger] Objet: {gameObject.name}");
            Debug.Log($"[InteractionDebugger] Tag: {gameObject.tag}");
            Debug.Log($"[InteractionDebugger] Layer: {LayerMask.LayerToName(gameObject.layer)}");
            
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                Debug.Log($"[InteractionDebugger] Collider Is Trigger: {col.isTrigger}");
            }
            else
            {
                Debug.LogError("[InteractionDebugger] PAS DE COLLIDER!");
            }
            
            InteractableObject io = GetComponent<InteractableObject>();
            if (io != null)
            {
                Debug.Log("[InteractionDebugger] ✓ InteractableObject trouvé");
            }
            else
            {
                Debug.LogWarning("[InteractionDebugger] ✗ InteractableObject manquant");
            }
        }
        
        void Update()
        {
            // Test direct de la touche
            if (Input.GetKeyDown(interactKey))
            {
                Debug.Log($"[InteractionDebugger] Touche {interactKey} pressée!");
                
                if (playerInRange)
                {
                    Debug.Log("[InteractionDebugger] Joueur dans la zone - Interaction possible!");
                    
                    // Flash visuel
                    if (showVisualFeedback && spriteRenderer != null)
                    {
                        spriteRenderer.color = Color.green;
                        Invoke("ResetColor", 0.2f);
                    }
                }
                else
                {
                    Debug.Log($"[InteractionDebugger] Joueur HORS zone - Distance: {distanceToPlayer:F2}");
                }
            }
            
            // Calculer distance au joueur
            if (currentPlayer == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    currentPlayer = player;
                }
            }
            
            if (currentPlayer != null)
            {
                distanceToPlayer = Vector2.Distance(transform.position, currentPlayer.transform.position);
            }
        }
        
        void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log($"[InteractionDebugger] OnTriggerEnter2D: {other.name} (Tag: {other.tag})");
            
            if (other.CompareTag("Player"))
            {
                playerInRange = true;
                currentPlayer = other.gameObject;
                Debug.Log("[InteractionDebugger] JOUEUR ENTRE DANS LA ZONE!");
                
                if (showVisualFeedback && spriteRenderer != null)
                {
                    spriteRenderer.color = Color.yellow;
                }
            }
        }
        
        void OnTriggerExit2D(Collider2D other)
        {
            Debug.Log($"[InteractionDebugger] OnTriggerExit2D: {other.name}");
            
            if (other.CompareTag("Player"))
            {
                playerInRange = false;
                Debug.Log("[InteractionDebugger] JOUEUR SORT DE LA ZONE!");
                
                if (showVisualFeedback && spriteRenderer != null)
                {
                    spriteRenderer.color = originalColor;
                }
            }
        }
        
        void ResetColor()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = playerInRange ? Color.yellow : originalColor;
            }
        }
        
        void OnDrawGizmos()
        {
            // Dessiner la zone d'interaction
            Gizmos.color = playerInRange ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position, 2f);
            
            // Ligne vers le joueur
            if (currentPlayer != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, currentPlayer.transform.position);
            }
        }
    }
}