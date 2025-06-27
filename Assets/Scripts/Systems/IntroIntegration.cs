using UnityEngine;
using UnityEngine.SceneManagement;
using EstiamGameJam2025;

namespace EstiamGameJam2025
{
    /// <summary>
    /// Script pour intégrer la scène d'intro de Quentin avec le GameManager principal
    /// Place ce script sur un GameObject dans SampleScene
    /// </summary>
    public class IntroIntegration : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Si true, charge IntroScene au démarrage")]
        public bool playIntroOnStart = true;
        
        [Tooltip("Nom de la scène d'intro")]
        public string introSceneName = "IntroScene";
        
        private static bool introPlayed = false;
        
        void Awake()
        {
            // Si l'intro n'a pas encore été jouée et qu'on veut la jouer
            if (!introPlayed && playIntroOnStart)
            {
                introPlayed = true;
                
                // Vérifier si la scène d'intro est dans les Build Settings
                if (Application.CanStreamedLevelBeLoaded(introSceneName))
                {
                    Debug.Log($"[IntroIntegration] Chargement de {introSceneName}");
                    SceneManager.LoadScene(introSceneName);
                }
                else
                {
                    Debug.LogWarning($"[IntroIntegration] La scène {introSceneName} n'est pas dans les Build Settings!");
                    // Continuer avec le jeu normal
                }
            }
        }
        
        /// <summary>
        /// Méthode pour réinitialiser l'état de l'intro (utile pour les tests)
        /// </summary>
        [ContextMenu("Reset Intro State")]
        public static void ResetIntroState()
        {
            introPlayed = false;
            Debug.Log("[IntroIntegration] État de l'intro réinitialisé");
        }
    }
}