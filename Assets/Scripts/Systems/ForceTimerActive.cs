using UnityEngine;
using EstiamGameJam2025;

namespace EstiamGameJam2025
{
    /// <summary>
    /// Force le timer à rester actif pendant le mini-jeu
    /// </summary>
    public class ForceTimerActive : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private bool forceTimerActive = true;
        [SerializeField] private bool autoStart = true;
        
        private TimeManager timeManager;
        private bool wasActive = false;
        
        void OnEnable()
        {
            timeManager = FindObjectOfType<TimeManager>();
            
            if (timeManager == null)
            {
                Debug.LogError("[ForceTimerActive] TimeManager non trouvé!");
                return;
            }
            
            // Sauvegarder l'état actuel
            wasActive = timeManager.IsCountingDown();
            
            if (forceTimerActive && autoStart && !wasActive)
            {
                Debug.Log("[ForceTimerActive] Redémarrage forcé du timer!");
                timeManager.ResumeTimer();
                
                // Si ResumeTimer ne marche pas, forcer avec StartCountdown
                if (!timeManager.IsCountingDown() && timeManager.GetRemainingTime() > 0)
                {
                    float remainingTime = timeManager.GetRemainingTime();
                    Debug.Log($"[ForceTimerActive] Force StartCountdown avec {remainingTime:F1}s");
                    timeManager.StartCountdown(remainingTime);
                }
            }
        }
        
        void Update()
        {
            if (forceTimerActive && timeManager != null)
            {
                // Vérifier chaque frame si le timer s'est arrêté
                if (!timeManager.IsCountingDown() && timeManager.GetRemainingTime() > 0)
                {
                    Debug.LogWarning("[ForceTimerActive] Timer arrêté! Redémarrage...");
                    timeManager.ResumeTimer();
                    
                    // Forcer si nécessaire
                    if (!timeManager.IsCountingDown())
                    {
                        float remainingTime = timeManager.GetRemainingTime();
                        timeManager.StartCountdown(remainingTime);
                    }
                }
            }
        }
        
        void OnDisable()
        {
            // Ne rien faire - laisser le timer continuer
        }
    }
}