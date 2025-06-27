using UnityEngine;
using EstiamGameJam2025;

namespace EstiamGameJam2025
{
    /// <summary>
    /// Détecte les changements de Time.timeScale qui pourraient arrêter le timer
    /// </summary>
    public class TimeScaleDebugger : MonoBehaviour
    {
        private float lastTimeScale = 1f;
        private GameState lastGameState;
        
        void Start()
        {
            lastTimeScale = Time.timeScale;
            if (GameManager.Instance != null)
            {
                lastGameState = GameManager.Instance.CurrentState;
            }
            
            Debug.Log($"[TimeScaleDebugger] Démarrage - TimeScale: {Time.timeScale}");
        }
        
        void Update()
        {
            // Vérifier Time.timeScale
            if (Mathf.Abs(Time.timeScale - lastTimeScale) > 0.01f)
            {
                Debug.LogWarning($"[TimeScaleDebugger] ⚠️ Time.timeScale changé : {lastTimeScale} → {Time.timeScale}");
                Debug.LogWarning($"[TimeScaleDebugger] Stack: {System.Environment.StackTrace}");
                lastTimeScale = Time.timeScale;
            }
            
            // Vérifier GameState
            if (GameManager.Instance != null)
            {
                GameState currentState = GameManager.Instance.CurrentState;
                if (currentState != lastGameState)
                {
                    Debug.Log($"[TimeScaleDebugger] 🎮 GameState changé : {lastGameState} → {currentState}");
                    lastGameState = currentState;
                }
            }
            
            // Afficher l'état quand on appuie sur E
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log($"[TimeScaleDebugger] === État au moment de E ===");
                Debug.Log($"[TimeScaleDebugger] Time.timeScale: {Time.timeScale}");
                Debug.Log($"[TimeScaleDebugger] GameState: {lastGameState}");
                
                var timeManager = FindObjectOfType<TimeManager>();
                if (timeManager != null)
                {
                    Debug.Log($"[TimeScaleDebugger] Timer actif: {timeManager.IsCountingDown()}");
                    Debug.Log($"[TimeScaleDebugger] Temps restant: {timeManager.GetRemainingTime():F1}s");
                }
            }
        }
        
        void OnGUI()
        {
            // Affichage visuel
            GUI.color = Time.timeScale == 0 ? Color.red : Color.green;
            GUI.Label(new Rect(10, 10, 200, 20), $"TimeScale: {Time.timeScale}");
            
            if (GameManager.Instance != null)
            {
                GUI.Label(new Rect(10, 30, 200, 20), $"GameState: {GameManager.Instance.CurrentState}");
            }
        }
    }
}