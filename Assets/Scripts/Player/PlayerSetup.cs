using UnityEngine;

namespace EstiamGameJam2025
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerSetup : MonoBehaviour
    {
        [Header("Collider Settings")]
        [SerializeField] private bool useCircleCollider = false;
        [SerializeField] private Vector2 boxColliderSize = new Vector2(0.8f, 0.8f);
        [SerializeField] private float circleColliderRadius = 0.4f;
        [SerializeField] private Vector2 colliderOffset = Vector2.zero;
        
        [Header("Rigidbody Settings")]
        [SerializeField] private float mass = 1f;
        [SerializeField] private float linearDrag = 0f;
        [SerializeField] private float angularDrag = 0.05f;
        
        void Awake()
        {
            SetupPlayer();
        }
        
        [ContextMenu("Setup Player Components")]
        public void SetupPlayer()
        {
            // Vérifier et configurer le Rigidbody2D
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
                Debug.Log("Rigidbody2D ajouté au joueur");
            }
            
            // Configurer le Rigidbody2D
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.mass = mass;
            rb.drag = linearDrag;
            rb.angularDrag = angularDrag;
            rb.gravityScale = 0f; // Pas de gravité pour un jeu vue du dessus
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Empêcher la rotation
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Pour éviter de traverser les murs
            rb.interpolation = RigidbodyInterpolation2D.Interpolate; // Pour un mouvement fluide
            
            // Vérifier et configurer le Collider2D
            Collider2D existingCollider = GetComponent<Collider2D>();
            
            if (existingCollider == null)
            {
                if (useCircleCollider)
                {
                    CircleCollider2D circleCollider = gameObject.AddComponent<CircleCollider2D>();
                    circleCollider.radius = circleColliderRadius;
                    circleCollider.offset = colliderOffset;
                    Debug.Log("CircleCollider2D ajouté au joueur");
                }
                else
                {
                    BoxCollider2D boxCollider = gameObject.AddComponent<BoxCollider2D>();
                    boxCollider.size = boxColliderSize;
                    boxCollider.offset = colliderOffset;
                    Debug.Log("BoxCollider2D ajouté au joueur");
                }
            }
            else
            {
                Debug.Log($"Le joueur a déjà un {existingCollider.GetType().Name}");
                
                // Configurer le collider existant
                if (existingCollider is BoxCollider2D box)
                {
                    box.size = boxColliderSize;
                    box.offset = colliderOffset;
                }
                else if (existingCollider is CircleCollider2D circle)
                {
                    circle.radius = circleColliderRadius;
                    circle.offset = colliderOffset;
                }
            }
            
            // Vérifier le layer
            if (gameObject.layer == 0) // Default layer
            {
                gameObject.layer = LayerSetup.GetLayerWithFallback("Player", "Default");
            }
            
            // Vérifier le tag
            if (string.IsNullOrEmpty(gameObject.tag) || gameObject.tag == "Untagged")
            {
                gameObject.tag = "Player";
                Debug.Log("Tag 'Player' assigné");
            }
            
            Debug.Log("Configuration du joueur terminée!");
        }
        
        void OnValidate()
        {
            // Limiter les valeurs dans l'éditeur
            boxColliderSize = new Vector2(Mathf.Max(0.1f, boxColliderSize.x), Mathf.Max(0.1f, boxColliderSize.y));
            circleColliderRadius = Mathf.Max(0.1f, circleColliderRadius);
            mass = Mathf.Max(0.1f, mass);
            linearDrag = Mathf.Max(0f, linearDrag);
            angularDrag = Mathf.Max(0f, angularDrag);
        }
        
        void OnDrawGizmosSelected()
        {
            // Visualiser le collider
            Gizmos.color = Color.green;
            
            if (useCircleCollider)
            {
                Gizmos.DrawWireSphere(transform.position + (Vector3)colliderOffset, circleColliderRadius);
            }
            else
            {
                Vector3 center = transform.position + (Vector3)colliderOffset;
                Gizmos.DrawWireCube(center, boxColliderSize);
            }
            
            // Visualiser la zone d'interaction
            PlayerController playerController = GetComponent<PlayerController>();
            if (playerController != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, 2f); // interactionRange par défaut
            }
        }
    }
}