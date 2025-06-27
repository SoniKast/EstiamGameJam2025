using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EstiamGameJam2025
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerAnimationController : MonoBehaviour
    {
        [Header("Animation Sprites")]
        [SerializeField] private Sprite[] idleUpSprites;
        [SerializeField] private Sprite[] idleDownSprites;
        [SerializeField] private Sprite[] idleLeftSprites;
        [SerializeField] private Sprite[] idleRightSprites;
        
        [SerializeField] private Sprite[] walkUpSprites;
        [SerializeField] private Sprite[] walkDownSprites;
        [SerializeField] private Sprite[] walkLeftSprites;
        [SerializeField] private Sprite[] walkRightSprites;
        
        [Header("Animation Settings")]
        [SerializeField] private float animationSpeed = 0.1f; // Temps entre chaque frame
        
        private SpriteRenderer spriteRenderer;
        private Sprite[] currentAnimation;
        private int currentFrame = 0;
        private float animationTimer = 0f;
        private bool isAnimating = false;
        
        public enum PlayerDirection
        {
            Up,
            Down,
            Left,
            Right
        }
        
        public enum PlayerState
        {
            Idle,
            Walking
        }
        
        private PlayerDirection currentDirection = PlayerDirection.Down;
        private PlayerState currentState = PlayerState.Idle;
        
        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            SetAnimation(PlayerState.Idle, PlayerDirection.Down);
        }
        
        private void Update()
        {
            if (isAnimating && currentAnimation != null && currentAnimation.Length > 0)
            {
                animationTimer += Time.deltaTime;
                
                if (animationTimer >= animationSpeed)
                {
                    animationTimer = 0f;
                    currentFrame = (currentFrame + 1) % currentAnimation.Length;
                    spriteRenderer.sprite = currentAnimation[currentFrame];
                }
            }
        }
        
        public void UpdateAnimation(Vector2 movement)
        {
            // Déterminer l'état (idle ou walking)
            PlayerState newState = movement.magnitude > 0.1f ? PlayerState.Walking : PlayerState.Idle;
            
            // Déterminer la direction
            PlayerDirection newDirection = currentDirection;
            
            if (movement.magnitude > 0.1f)
            {
                // Priorité : Horizontal > Vertical
                if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
                {
                    newDirection = movement.x > 0 ? PlayerDirection.Right : PlayerDirection.Left;
                }
                else
                {
                    newDirection = movement.y > 0 ? PlayerDirection.Up : PlayerDirection.Down;
                }
            }
            
            // Changer l'animation si nécessaire
            if (newState != currentState || newDirection != currentDirection)
            {
                SetAnimation(newState, newDirection);
            }
        }
        
        private void SetAnimation(PlayerState state, PlayerDirection direction)
        {
            currentState = state;
            currentDirection = direction;
            currentFrame = 0;
            animationTimer = 0f;
            
            // Sélectionner la bonne animation
            if (state == PlayerState.Idle)
            {
                switch (direction)
                {
                    case PlayerDirection.Up:
                        currentAnimation = idleUpSprites;
                        break;
                    case PlayerDirection.Down:
                        currentAnimation = idleDownSprites;
                        break;
                    case PlayerDirection.Left:
                        currentAnimation = idleLeftSprites;
                        break;
                    case PlayerDirection.Right:
                        currentAnimation = idleRightSprites;
                        break;
                }
            }
            else // Walking
            {
                switch (direction)
                {
                    case PlayerDirection.Up:
                        currentAnimation = walkUpSprites;
                        break;
                    case PlayerDirection.Down:
                        currentAnimation = walkDownSprites;
                        break;
                    case PlayerDirection.Left:
                        currentAnimation = walkLeftSprites;
                        break;
                    case PlayerDirection.Right:
                        currentAnimation = walkRightSprites;
                        break;
                }
            }
            
            // Commencer l'animation
            if (currentAnimation != null && currentAnimation.Length > 0)
            {
                spriteRenderer.sprite = currentAnimation[0];
                isAnimating = currentAnimation.Length > 1;
            }
        }
        
        public void StopAnimation()
        {
            isAnimating = false;
            SetAnimation(PlayerState.Idle, currentDirection);
        }
        
        public void PlaySpecialAnimation(Sprite[] specialAnimation)
        {
            if (specialAnimation != null && specialAnimation.Length > 0)
            {
                currentAnimation = specialAnimation;
                currentFrame = 0;
                animationTimer = 0f;
                isAnimating = true;
                spriteRenderer.sprite = currentAnimation[0];
            }
        }
    }
}