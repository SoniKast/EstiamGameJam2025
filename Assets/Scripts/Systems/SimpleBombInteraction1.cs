using UnityEngine;
using UnityEngine.Events;

namespace EstiamGameJam2025
{
    /// <summary>
    /// Script simple pour activer un prefab quand on interagit avec la bombe
    /// </summary>
    public class SimpleBombInteraction : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] private string interactionPrompt = "Appuyez sur E pour désamorcer";
        [SerializeField] private bool disableAfterUse = true;
        
        [Header("Prefab à activer")]
        [SerializeField] private GameObject bombPrefabToActivate;
        [SerializeField] private bool instantiateNewPrefab = false; // Si false, active un prefab existant dans la scène
        
        [Header("Options")]
        [SerializeField] private bool pausePlayerDuringMinigame = true;
        [SerializeField] private bool hideBombDuringMinigame = true;
        
        [Header("Events")]
        public UnityEvent onPlayerEnterRange;
        public UnityEvent onPlayerExitRange;
        public UnityEvent onInteract;
        public UnityEvent onMinigameComplete;
        
        private bool isPlayerInRange = false;
        private bool hasBeenUsed = false;
        private GameObject player;
        private GameObject activatedPrefab;
        private SpriteRenderer spriteRenderer;
        
        void Start()
        {
            // Configuration du collider
            Collider2D col = GetComponent<Collider2D>();
            if (col == null)
            {
                col = gameObject.AddComponent<BoxCollider2D>();
            }
            col.isTrigger = true;
            
            // Configuration du tag
            if (gameObject.tag != "Interactable")
            {
                gameObject.tag = "Interactable";
            }
            
            // Trouver le joueur
            player = GameObject.FindGameObjectWithTag("Player");
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            // Si le prefab est déjà dans la scène, le désactiver au début
            if (!instantiateNewPrefab && bombPrefabToActivate != null)
            {
                bombPrefabToActivate.SetActive(false);
            }
        }
        
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !hasBeenUsed)
            {
                isPlayerInRange = true;
                onPlayerEnterRange?.Invoke();
                Debug.Log($"[SimpleBombInteraction] Joueur proche - {interactionPrompt}");
            }
        }
        
        void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerInRange = false;
                onPlayerExitRange?.Invoke();
            }
        }
        
        void Update()
        {
            if (isPlayerInRange && !hasBeenUsed && Input.GetKeyDown(KeyCode.E))
            {
                Interact();
            }
        }
        
        public void Interact()
        {
            if (hasBeenUsed && disableAfterUse) return;
            
            Debug.Log($"[SimpleBombInteraction] Activation du mini-jeu bombe");
            
            onInteract?.Invoke();
            
            // Activer le prefab
            if (bombPrefabToActivate != null)
            {
                if (instantiateNewPrefab)
                {
                    // Créer une nouvelle instance
                    activatedPrefab = Instantiate(bombPrefabToActivate, transform.position, Quaternion.identity);
                }
                else
                {
                    // Activer le prefab existant
                    bombPrefabToActivate.SetActive(true);
                    activatedPrefab = bombPrefabToActivate;
                }
                
                // Gérer le joueur
                if (pausePlayerDuringMinigame && player != null)
                {
                    PlayerController pc = player.GetComponent<PlayerController>();
                    if (pc != null)
                    {
                        pc.SetCanMove(false);
                    }
                }
                
                // Cacher la bombe
                if (hideBombDuringMinigame && spriteRenderer != null)
                {
                    spriteRenderer.enabled = false;
                }
                
                if (disableAfterUse)
                {
                    hasBeenUsed = true;
                }
            }
            else
            {
                Debug.LogError("[SimpleBombInteraction] Aucun prefab assigné!");
            }
        }
        
        /// <summary>
        /// Appeler cette méthode quand le mini-jeu est terminé
        /// </summary>
        public void OnMinigameFinished(bool success = true)
        {
            Debug.Log($"[SimpleBombInteraction] Mini-jeu terminé - Succès: {success}");
            
            // Réactiver le joueur
            if (pausePlayerDuringMinigame && player != null)
            {
                PlayerController pc = player.GetComponent<PlayerController>();
                if (pc != null)
                {
                    pc.SetCanMove(true);
                }
            }
            
            // Désactiver/détruire le prefab
            if (activatedPrefab != null)
            {
                if (instantiateNewPrefab)
                {
                    Destroy(activatedPrefab);
                }
                else
                {
                    activatedPrefab.SetActive(false);
                }
            }
            
            // Événement de fin
            onMinigameComplete?.Invoke();
            
            if (success)
            {
                // La bombe a été désamorcée
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = Color.green;
                    spriteRenderer.enabled = true;
                }
            }
            else
            {
                // Échec - permettre de réessayer
                hasBeenUsed = false;
                if (spriteRenderer != null)
                {
                    spriteRenderer.enabled = true;
                }
            }
        }
        
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 2f);
        }
    }
}