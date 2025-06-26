using UnityEngine;
using UnityEngine.Events;

namespace EstiamGameJam2025
{
    public class InteractableObject : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] private string interactionPrompt = "Appuyez sur E pour interagir";
        [SerializeField] private float interactionRange = 2f;
        [SerializeField] private bool requirePlayerFacing = false;
        
        [Header("Visual Feedback")]
        [SerializeField] private bool showOutlineOnHover = true;
        [SerializeField] private Color outlineColor = Color.yellow;
        [SerializeField] private float outlineWidth = 0.1f;
        [SerializeField] private GameObject promptUI; // UI qui s'affiche quand le joueur est proche
        
        [Header("Mini-Game Configuration")]
        [SerializeField] private MiniGameType miniGameToStart = MiniGameType.SwitchActivation;
        [SerializeField] private bool disableAfterUse = true;
        [SerializeField] private bool requireMiniGameSuccess = true;
        
        [Header("Events")]
        public UnityEvent onPlayerEnterRange;
        public UnityEvent onPlayerExitRange;
        public UnityEvent onInteract;
        public UnityEvent onMiniGameSuccess;
        public UnityEvent onMiniGameFail;
        
        private bool isPlayerInRange = false;
        private bool hasBeenUsed = false;
        private SpriteRenderer spriteRenderer;
        private GameObject player;
        private Material originalMaterial;
        private Material outlineMaterial;
        
        void Start()
        {
            // Configuration du collider
            Collider2D col = GetComponent<Collider2D>();
            if (col == null)
            {
                // Ajouter automatiquement un BoxCollider2D si aucun collider n'existe
                col = gameObject.AddComponent<BoxCollider2D>();
                Debug.Log("BoxCollider2D ajouté automatiquement à " + gameObject.name);
            }
            col.isTrigger = true;
            
            // Configuration du layer
            gameObject.layer = LayerSetup.GetLayerWithFallback("Interactable", "Default");
            gameObject.tag = "Interactable";
            
            // Configuration visuelle
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && showOutlineOnHover)
            {
                originalMaterial = spriteRenderer.material;
                CreateOutlineMaterial();
            }
            
            // Cacher le prompt au début
            if (promptUI != null)
                promptUI.SetActive(false);
            
            // Trouver le joueur
            player = GameObject.FindGameObjectWithTag("Player");
            
            // S'abonner aux événements du MiniGameManager
            MiniGameManager miniGameManager = FindObjectOfType<MiniGameManager>();
            if (miniGameManager != null)
            {
                miniGameManager.onMiniGameComplete += OnMiniGameComplete;
            }
        }
        
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !hasBeenUsed)
            {
                isPlayerInRange = true;
                ShowInteractionFeedback(true);
                onPlayerEnterRange?.Invoke();
            }
        }
        
        void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerInRange = false;
                ShowInteractionFeedback(false);
                onPlayerExitRange?.Invoke();
            }
        }
        
        void Update()
        {
            // Vérifier l'interaction
            if (isPlayerInRange && !hasBeenUsed && Input.GetKeyDown(KeyCode.E))
            {
                Interact();
            }
            
            // Mise à jour du prompt UI pour qu'il suive l'objet
            if (promptUI != null && promptUI.activeSelf)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.5f);
                promptUI.transform.position = screenPos;
            }
        }
        
        void Interact()
        {
            Debug.Log($"Interaction avec {gameObject.name}");
            
            onInteract?.Invoke();
            
            // Démarrer le mini-jeu
            MiniGameManager miniGameManager = FindObjectOfType<MiniGameManager>();
            if (miniGameManager != null)
            {
                miniGameManager.StartMiniGame(miniGameToStart);
                
                if (disableAfterUse)
                {
                    hasBeenUsed = true;
                    ShowInteractionFeedback(false);
                }
            }
            else
            {
                Debug.LogError("MiniGameManager non trouvé!");
            }
        }
        
        void ShowInteractionFeedback(bool show)
        {
            // Afficher/Cacher le prompt
            if (promptUI != null)
            {
                promptUI.SetActive(show && !hasBeenUsed);
            }
            
            // Effet outline
            if (spriteRenderer != null && showOutlineOnHover)
            {
                spriteRenderer.material = show ? outlineMaterial : originalMaterial;
            }
            
            // Effet de pulsation
            if (show)
            {
                StartCoroutine(PulseEffect());
            }
            else
            {
                StopAllCoroutines();
                transform.localScale = Vector3.one;
            }
        }
        
        System.Collections.IEnumerator PulseEffect()
        {
            float time = 0;
            Vector3 originalScale = transform.localScale;
            
            while (isPlayerInRange)
            {
                time += Time.deltaTime;
                float scale = 1f + Mathf.Sin(time * 3f) * 0.05f;
                transform.localScale = originalScale * scale;
                yield return null;
            }
            
            transform.localScale = originalScale;
        }
        
        void CreateOutlineMaterial()
        {
            // Créer un matériau avec outline
            outlineMaterial = new Material(Shader.Find("Sprites/Default"));
            outlineMaterial.SetFloat("_OutlineWidth", outlineWidth);
            outlineMaterial.SetColor("_OutlineColor", outlineColor);
        }
        
        void OnMiniGameComplete(bool success)
        {
            if (!isPlayerInRange) return;
            
            if (success)
            {
                onMiniGameSuccess?.Invoke();
                Debug.Log($"{gameObject.name} : Mini-jeu réussi!");
                
                // Peut-être détruire l'objet ou le marquer comme complété
                if (requireMiniGameSuccess)
                {
                    spriteRenderer.color = Color.green;
                }
            }
            else
            {
                onMiniGameFail?.Invoke();
                Debug.Log($"{gameObject.name} : Mini-jeu échoué!");
                
                // Permettre de réessayer
                if (requireMiniGameSuccess)
                {
                    hasBeenUsed = false;
                }
            }
        }
        
        void OnDrawGizmosSelected()
        {
            // Visualiser la zone d'interaction
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRange);
        }
        
        void OnDestroy()
        {
            // Se désabonner des événements
            MiniGameManager miniGameManager = FindObjectOfType<MiniGameManager>();
            if (miniGameManager != null)
            {
                miniGameManager.onMiniGameComplete -= OnMiniGameComplete;
            }
        }
    }
}