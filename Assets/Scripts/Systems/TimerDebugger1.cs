using UnityEngine;
using EstiamGameJam2025;

namespace EstiamGameJam2025
{
    /// <summary>
    /// Debug le timer pour comprendre pourquoi il s'arrête
    /// </summary>
    public class TimerDebugger : MonoBehaviour
    {
        private TimeManager timeManager;
        private float lastTime = -1f;
        private bool wasCountingDown = false;
        
        void Start()
        {
            timeManager = FindObjectOfType<TimeManager>();
            if (timeManager == null)
            {
                Debug.LogError("[TimerDebugger] TimeManager non trouvé!");
            }
        }
        
        void Update()
        {
            if (timeManager == null) return;
            
            float currentTime = timeManager.GetRemainingTime();
            bool isCountingDown = timeManager.IsCountingDown();
            
            // Détecter les changements d'état
            if (isCountingDown != wasCountingDown)
            {
                if (isCountingDown)
                {
                    Debug.Log($"[TimerDebugger] ✅ Timer DÉMARRÉ - Temps: {currentTime:F1}s");
                }
                else
                {
                    Debug.Log($"[TimerDebugger] ⏸️ Timer ARRÊTÉ - Temps: {currentTime:F1}s");
                    Debug.Log("[TimerDebugger] Stack trace:");
                    Debug.Log(System.Environment.StackTrace);
                }
                wasCountingDown = isCountingDown;
            }
            
            // Vérifier si le temps change
            if (Mathf.Abs(currentTime - lastTime) > 0.1f)
            {
                if (lastTime >= 0 && currentTime > lastTime)
                {
                    Debug.Log($"[TimerDebugger] ⏪ Temps AUGMENTÉ: {lastTime:F1}s → {currentTime:F1}s");
                }
                lastTime = currentTime;
            }
            
            // Afficher l'état du GameManager
            if (GameManager.Instance != null && Input.GetKeyDown(KeyCode.F1))
            {
                Debug.Log($"[TimerDebugger] État du jeu: {GameManager.Instance.CurrentState}");
                Debug.Log($"[TimerDebugger] Timer actif: {isCountingDown}");
                Debug.Log($"[TimerDebugger] Temps restant: {currentTime:F1}s");
            }
        }
        
        void OnGUI()
        {
            if (timeManager == null) return;
            
            // Afficher l'état en haut à droite
            GUI.color = timeManager.IsCountingDown() ? Color.green : Color.red;
            string status = timeManager.IsCountingDown() ? "ACTIF" : "PAUSE";
            GUI.Label(new Rect(Screen.width - 150, 10, 140, 30), 
                $"Timer: {status}\nTemps: {timeManager.GetRemainingTime():F1}s");
            
            // État du GameManager
            if (GameManager.Instance != null)
            {
                GUI.Label(new Rect(Screen.width - 150, 50, 140, 20), 
                    $"État: {GameManager.Instance.CurrentState}");
            }
        }
    }
}