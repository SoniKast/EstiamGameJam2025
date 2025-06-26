using UnityEngine;

namespace EstiamGameJam2025
{
    public class CollisionDebugger : MonoBehaviour
    {
        [Header("Debug Settings")]
        [SerializeField] private bool showDebugInfo = true;
        [SerializeField] private bool logCollisions = true;
        [SerializeField] private Color collisionColor = Color.red;
        [SerializeField] private float debugDuration = 0.5f;
        
        private Rigidbody2D rb;
        private Collider2D col;
        
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
            
            if (rb == null)
            {
                Debug.LogError($"[CollisionDebugger] {gameObject.name} n'a pas de Rigidbody2D!");
            }
            
            if (col == null)
            {
                Debug.LogError($"[CollisionDebugger] {gameObject.name} n'a pas de Collider2D!");
            }
            else
            {
                Debug.Log($"[CollisionDebugger] {gameObject.name} a un {col.GetType().Name}");
                Debug.Log($"[CollisionDebugger] Layer: {LayerMask.LayerToName(gameObject.layer)} ({gameObject.layer})");
                Debug.Log($"[CollisionDebugger] Tag: {gameObject.tag}");
                
                if (col.isTrigger)
                {
                    Debug.LogWarning($"[CollisionDebugger] Le collider est en mode Trigger! Les collisions physiques ne fonctionneront pas.");
                }
            }
            
            if (rb != null)
            {
                Debug.Log($"[CollisionDebugger] Rigidbody2D - Type: {rb.bodyType}, GravityScale: {rb.gravityScale}");
                Debug.Log($"[CollisionDebugger] Collision Detection: {rb.collisionDetectionMode}");
                Debug.Log($"[CollisionDebugger] Constraints: {rb.constraints}");
            }
        }
        
        void OnCollisionEnter2D(Collision2D collision)
        {
            if (logCollisions)
            {
                Debug.Log($"[Collision] {gameObject.name} a touché {collision.gameObject.name} (Layer: {LayerMask.LayerToName(collision.gameObject.layer)})");
                
                // Afficher le point de contact
                if (collision.contactCount > 0)
                {
                    ContactPoint2D contact = collision.GetContact(0);
                    Debug.DrawRay(contact.point, contact.normal * 2f, collisionColor, debugDuration);
                }
            }
        }
        
        void OnCollisionStay2D(Collision2D collision)
        {
            if (showDebugInfo && collision.contactCount > 0)
            {
                ContactPoint2D contact = collision.GetContact(0);
                Debug.DrawRay(contact.point, contact.normal, collisionColor);
            }
        }
        
        void OnTriggerEnter2D(Collider2D other)
        {
            if (logCollisions)
            {
                Debug.Log($"[Trigger] {gameObject.name} est entré dans le trigger de {other.gameObject.name}");
            }
        }
        
        void OnDrawGizmos()
        {
            if (!showDebugInfo) return;
            
            // Afficher la vélocité
            if (rb != null && Application.isPlaying)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(transform.position, rb.velocity);
            }
            
            // Afficher les bounds du collider
            if (col != null)
            {
                Bounds bounds = col.bounds;
                Gizmos.color = new Color(0, 1, 0, 0.3f);
                Gizmos.DrawCube(bounds.center, bounds.size);
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(bounds.center, bounds.size);
            }
        }
    }
}