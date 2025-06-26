using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EstiamGameJam2025
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float acceleration = 10f;
        [SerializeField] private float deceleration = 10f;

        [Header("Interaction Settings")]
        [SerializeField] private float interactionRange = 2f;
        [SerializeField] private LayerMask interactableLayer;
        [SerializeField] private KeyCode interactKey = KeyCode.E;

        [Header("Visual Settings")]
        [SerializeField] private bool flipSpriteWithDirection = true;

        // Components
        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;
        private Animator animator;
        private PlayerAnimationController animationController;

        // Movement
        private Vector2 moveInput;
        private Vector2 currentVelocity;
        private bool canMove = true;

        // Interaction
        private GameObject currentInteractable;
        private bool isInteracting = false;

        // State
        private bool isRewinding = false;
        private List<PlayerFrame> recordedFrames = new List<PlayerFrame>();

        // Structure pour enregistrer l'état du joueur
        [System.Serializable]
        private struct PlayerFrame
        {
            public Vector3 position;
            public Quaternion rotation;
            public bool isFacingRight;
            public float timestamp;

            public PlayerFrame(Vector3 pos, Quaternion rot, bool facingRight, float time)
            {
                position = pos;
                rotation = rot;
                isFacingRight = facingRight;
                timestamp = time;
            }
        }

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            animationController = GetComponent<PlayerAnimationController>();

            // S'abonner aux changements d'état du jeu
            if (GameManager.Instance != null)
            {
                GameManager.Instance.onGameStateChanged += OnGameStateChanged;
            }
        }

        private void Update()
        {
            if (!canMove || isRewinding) return;

            HandleMovementInput();
            HandleInteraction();
            CheckForInteractables();

            // Enregistrer la position pour le rewind
            if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Playing)
            {
                RecordFrame();
            }
        }

        private void FixedUpdate()
        {
            if (!canMove || isRewinding) return;

            ApplyMovement();
        }

        private void HandleMovementInput()
        {
            // Récupérer les inputs
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            moveInput = new Vector2(horizontal, vertical).normalized;

            // Animation avec le nouveau système
            if (animationController != null)
            {
                animationController.UpdateAnimation(moveInput);
            }
            // Sinon utiliser l'ancien système Animator
            else if (animator != null)
            {
                animator.SetFloat("MoveSpeed", moveInput.magnitude);
            }

            // Flip du sprite selon la direction (seulement si pas de animationController)
            if (animationController == null && flipSpriteWithDirection && moveInput.x != 0)
            {
                bool shouldFaceRight = moveInput.x > 0;
                if (spriteRenderer != null)
                {
                    spriteRenderer.flipX = !shouldFaceRight;
                }
            }
        }

        private void ApplyMovement()
        {
            if (moveInput.magnitude > 0)
            {
                // Accélération
                currentVelocity = Vector2.MoveTowards(currentVelocity, moveInput * moveSpeed, acceleration * Time.fixedDeltaTime);
            }
            else
            {
                // Décélération
                currentVelocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
            }

            rb.velocity = currentVelocity;
        }

        private void HandleInteraction()
        {
            if (Input.GetKeyDown(interactKey) && currentInteractable != null && !isInteracting)
            {
                InteractWithObject();
            }
        }

        private void CheckForInteractables()
        {
            // Chercher les objets interactables proches
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactionRange, interactableLayer);
            
            GameObject closestInteractable = null;
            float closestDistance = float.MaxValue;

            foreach (Collider2D col in colliders)
            {
                float distance = Vector2.Distance(transform.position, col.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestInteractable = col.gameObject;
                }
            }

            // Mettre à jour l'objet interactable actuel
            if (currentInteractable != closestInteractable)
            {
                if (currentInteractable != null)
                {
                    // Désactiver le highlight de l'ancien objet
                    OnInteractableExit(currentInteractable);
                }

                currentInteractable = closestInteractable;

                if (currentInteractable != null)
                {
                    // Activer le highlight du nouvel objet
                    OnInteractableEnter(currentInteractable);
                }
            }
        }

        private void InteractWithObject()
        {
            if (currentInteractable == null) return;

            isInteracting = true;
            Debug.Log("Interacting with: " + currentInteractable.name);

            // Vérifier si c'est un déclencheur de mini-jeu
            MiniGameTrigger trigger = currentInteractable.GetComponent<MiniGameTrigger>();
            if (trigger != null)
            {
                GameManager.Instance.StartMiniGame();
            }

            // Autres types d'interactions peuvent être ajoutés ici

            StartCoroutine(Co_InteractionCooldown());
        }

        private IEnumerator Co_InteractionCooldown()
        {
            yield return new WaitForSeconds(0.5f);
            isInteracting = false;
        }

        private void OnInteractableEnter(GameObject interactable)
        {
            Debug.Log("Can interact with: " + interactable.name + " (Press " + interactKey + ")");
            // Ajouter un effet visuel (outline, highlight, etc.)
        }

        private void OnInteractableExit(GameObject interactable)
        {
            // Retirer l'effet visuel
        }

        private void RecordFrame()
        {
            // Limiter le nombre de frames enregistrées
            const int maxFrames = 600; // ~10 secondes à 60 FPS
            
            if (recordedFrames.Count >= maxFrames)
            {
                recordedFrames.RemoveAt(0);
            }

            bool isFacingRight = spriteRenderer != null ? !spriteRenderer.flipX : true;
            PlayerFrame frame = new PlayerFrame(
                transform.position,
                transform.rotation,
                isFacingRight,
                Time.time
            );

            recordedFrames.Add(frame);
        }

        public void StartRewind()
        {
            isRewinding = true;
            canMove = false;
            rb.velocity = Vector2.zero;
            StartCoroutine(Co_RewindMovement());
        }

        private IEnumerator Co_RewindMovement()
        {
            int frameIndex = recordedFrames.Count - 1;
            float rewindSpeed = 2f; // Vitesse du rewind

            while (frameIndex >= 0 && isRewinding)
            {
                PlayerFrame frame = recordedFrames[frameIndex];
                
                // Appliquer la position et rotation
                transform.position = frame.position;
                transform.rotation = frame.rotation;
                
                // Appliquer la direction du sprite
                if (spriteRenderer != null)
                {
                    spriteRenderer.flipX = !frame.isFacingRight;
                }

                frameIndex--;
                yield return new WaitForSeconds(1f / (60f * rewindSpeed));
            }

            isRewinding = false;
        }

        private void OnGameStateChanged(GameState newState)
        {
            switch (newState)
            {
                case GameState.Playing:
                case GameState.Investigating:
                    canMove = true;
                    isRewinding = false;
                    break;
                case GameState.Rewinding:
                    StartRewind();
                    break;
                case GameState.MiniGame:
                case GameState.GameOver:
                case GameState.Victory:
                    canMove = false;
                    rb.velocity = Vector2.zero;
                    if (animationController != null)
                    {
                        animationController.StopAnimation();
                    }
                    break;
            }
        }

        public void SetCanMove(bool value)
        {
            canMove = value;
            if (!canMove)
            {
                rb.velocity = Vector2.zero;
                moveInput = Vector2.zero;
            }
        }

        // Debug
        private void OnDrawGizmosSelected()
        {
            // Dessiner la zone d'interaction
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRange);
        }

        private void OnDestroy()
        {
            // Se désabonner des événements
            if (GameManager.Instance != null)
            {
                GameManager.Instance.onGameStateChanged -= OnGameStateChanged;
            }
        }
    }
}