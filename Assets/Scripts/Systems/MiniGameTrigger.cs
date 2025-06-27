using UnityEngine;

namespace EstiamGameJam2025
{
    public class MiniGameTrigger : MonoBehaviour
    {
        [SerializeField] private MiniGameType _miniGameType;
        
        public MiniGameType MiniGameType 
        { 
            get => _miniGameType; 
            set => _miniGameType = value; 
        }
        
        void Start()
        {
            // S'assurer que l'objet est bien configuré
            if (gameObject.tag != "Interactable")
            {
                gameObject.tag = "Interactable";
            }
            
            // S'assurer qu'il y a un collider
            Collider2D col2D = GetComponent<Collider2D>();
            Collider col3D = GetComponent<Collider>();
            
            if (col2D == null && col3D == null)
            {
                // Ajouter un BoxCollider2D par défaut pour un jeu 2D
                col2D = gameObject.AddComponent<BoxCollider2D>();
                col2D.isTrigger = true;
            }
        }
        
        public void Interact()
        {
            // Démarrer le mini-jeu correspondant
            MiniGameManager miniGameManager = FindObjectOfType<MiniGameManager>();
            if (miniGameManager != null)
            {
                Debug.Log($"Démarrage du mini-jeu : {_miniGameType}");
                miniGameManager.StartMiniGame(_miniGameType);
            }
            else
            {
                Debug.LogError("MiniGameManager non trouvé !");
            }
        }
    }
}