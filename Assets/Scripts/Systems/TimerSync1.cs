using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EstiamGameJam2025;

namespace EstiamGameJam2025
{
    /// <summary>
    /// Synchronise le timer principal avec le timer du mini-jeu
    /// </summary>
    public class TimerSync : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private bool syncOnEnable = true;
        [SerializeField] private bool hideMainTimerDuringMinigame = true;
        [SerializeField] private bool pauseMainTimer = false; // Par défaut, le timer continue !
        
        [Header("Timer du Mini-jeu (Optionnel)")]
        [SerializeField] private TextMeshProUGUI minigameTimerText;
        [SerializeField] private Text minigameTimerTextLegacy; // Si ton collègue utilise Text normal
        [SerializeField] private float updateInterval = 0.1f;
        
        private TimeManager mainTimeManager;
        private GameObject mainTimerUI;
        private float lastUpdateTime;
        private bool isSyncing = false;
        
        void OnEnable()
        {
            if (syncOnEnable)
            {
                StartSync();
            }
        }
        
        void OnDisable()
        {
            StopSync();
        }
        
        public void StartSync()
        {
            // Trouver le TimeManager principal
            mainTimeManager = FindObjectOfType<TimeManager>();
            if (mainTimeManager == null)
            {
                Debug.LogError("[TimerSync] TimeManager principal non trouvé!");
                return;
            }
            
            // Récupérer l'UI du timer principal
            var timerUI = GameObject.Find("TimerBackground") ?? GameObject.Find("TimerUI");
            if (timerUI != null)
            {
                mainTimerUI = timerUI;
                if (hideMainTimerDuringMinigame)
                {
                    mainTimerUI.SetActive(false);
                }
            }
            
            // Pauser le timer principal si demandé
            if (pauseMainTimer)
            {
                mainTimeManager.PauseTimer();
            }
            
            isSyncing = true;
            Debug.Log("[TimerSync] Synchronisation démarrée");
        }
        
        public void StopSync()
        {
            if (!isSyncing) return;
            
            // Réactiver le timer principal
            if (mainTimerUI != null && hideMainTimerDuringMinigame)
            {
                mainTimerUI.SetActive(true);
            }
            
            // Reprendre le timer si on l'avait pausé
            if (pauseMainTimer && mainTimeManager != null)
            {
                mainTimeManager.ResumeTimer();
            }
            
            isSyncing = false;
            Debug.Log("[TimerSync] Synchronisation arrêtée");
        }
        
        void Update()
        {
            if (!isSyncing || mainTimeManager == null) return;
            
            // Mettre à jour seulement à intervalles réguliers
            if (Time.time - lastUpdateTime < updateInterval) return;
            lastUpdateTime = Time.time;
            
            // Récupérer le temps restant
            float remainingTime = mainTimeManager.GetRemainingTime();
            
            // Formater le temps
            int minutes = Mathf.FloorToInt(remainingTime / 60f);
            int seconds = Mathf.FloorToInt(remainingTime % 60f);
            string timeString = string.Format("{0:00}:{1:00}", minutes, seconds);
            
            // Mettre à jour le texte du mini-jeu
            if (minigameTimerText != null)
            {
                minigameTimerText.text = timeString;
                
                // Copier les couleurs du timer principal
                if (remainingTime <= 10f)
                    minigameTimerText.color = Color.red;
                else if (remainingTime <= 20f)
                    minigameTimerText.color = Color.yellow;
                else
                    minigameTimerText.color = Color.white;
            }
            
            // Support pour Text legacy
            if (minigameTimerTextLegacy != null)
            {
                minigameTimerTextLegacy.text = timeString;
                
                if (remainingTime <= 10f)
                    minigameTimerTextLegacy.color = Color.red;
                else if (remainingTime <= 20f)
                    minigameTimerTextLegacy.color = Color.yellow;
                else
                    minigameTimerTextLegacy.color = Color.white;
            }
        }
        
        /// <summary>
        /// Méthode pour déduire du temps (appelée par le mini-jeu en cas d'erreur)
        /// </summary>
        public void RemoveTime(float seconds)
        {
            if (mainTimeManager != null)
            {
                mainTimeManager.AddTime(-seconds);
                Debug.Log($"[TimerSync] -{seconds} secondes retirées");
            }
        }
        
        /// <summary>
        /// Méthode pour ajouter du temps (appelée par le mini-jeu en cas de succès)
        /// </summary>
        public void AddBonusTime(float seconds)
        {
            if (mainTimeManager != null)
            {
                mainTimeManager.AddTime(seconds);
                Debug.Log($"[TimerSync] +{seconds} secondes bonus!");
            }
        }
        
        /// <summary>
        /// Vérifie s'il reste du temps
        /// </summary>
        public bool HasTimeRemaining()
        {
            return mainTimeManager != null && mainTimeManager.GetRemainingTime() > 0;
        }
        
        /// <summary>
        /// Appelée quand le mini-jeu est terminé
        /// </summary>
        public void OnMinigameComplete(bool success)
        {
            StopSync();
            
            // Bonus/Malus de temps selon le résultat
            if (success)
            {
                AddBonusTime(10f); // +10 secondes en cas de succès
            }
            else
            {
                RemoveTime(5f); // -5 secondes en cas d'échec
            }
        }
    }
}