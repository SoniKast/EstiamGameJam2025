using UnityEngine;
using EstiamGameJam2025;

namespace EstiamGameJam2025
{
    /// <summary>
    /// Script simple pour démarrer le timer manuellement sans dépendre du GameManager
    /// </summary>
    public class SimpleTimerStarter : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private bool startOnAwake = true;
        [SerializeField] private float timerDuration = 60f;
        [SerializeField] private float startDelay = 0.5f;
        
        private TimeManager timeManager;
        
        void Start()
        {
            // Trouver le TimeManager
            timeManager = FindObjectOfType<TimeManager>();
            
            if (timeManager == null)
            {
                Debug.LogError("[SimpleTimerStarter] TimeManager non trouvé!");
                return;
            }
            
            if (startOnAwake)
            {
                // Démarrer le timer après un court délai
                Invoke(nameof(StartTimer), startDelay);
            }
        }
        
        [ContextMenu("Start Timer Now")]
        public void StartTimer()
        {
            if (timeManager != null)
            {
                Debug.Log($"[SimpleTimerStarter] Démarrage du timer : {timerDuration} secondes");
                timeManager.StartCountdown(timerDuration);
            }
            else
            {
                Debug.LogError("[SimpleTimerStarter] TimeManager non trouvé!");
            }
        }
        
        [ContextMenu("Stop Timer")]
        public void StopTimer()
        {
            if (timeManager != null)
            {
                timeManager.StopCountdown();
            }
        }
        
        [ContextMenu("Add 10 Seconds")]
        public void AddTime()
        {
            if (timeManager != null)
            {
                timeManager.AddTime(10f);
                Debug.Log("[SimpleTimerStarter] +10 secondes ajoutées");
            }
        }
    }
}