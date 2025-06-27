using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EstiamGameJam2025;

namespace EstiamGameJam2025
{
    /// <summary>
    /// Affiche simplement le timer principal sans jamais le pauser
    /// </summary>
    public class SimpleTimerDisplay : MonoBehaviour
    {
        [Header("Affichage du timer")]
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private Text timerTextLegacy;
        
        private TimeManager mainTimeManager;
        
        void Start()
        {
            // Trouver le TimeManager
            mainTimeManager = FindObjectOfType<TimeManager>();
            if (mainTimeManager == null)
            {
                Debug.LogError("[SimpleTimerDisplay] TimeManager non trouvé!");
            }
        }
        
        void Update()
        {
            if (mainTimeManager == null) return;
            
            // Récupérer le temps
            float remainingTime = mainTimeManager.GetRemainingTime();
            
            // Formater
            int minutes = Mathf.FloorToInt(remainingTime / 60f);
            int seconds = Mathf.FloorToInt(remainingTime % 60f);
            string timeString = string.Format("{0:00}:{1:00}", minutes, seconds);
            
            // Afficher
            if (timerText != null)
            {
                timerText.text = timeString;
                UpdateColor(timerText, remainingTime);
            }
            
            if (timerTextLegacy != null)
            {
                timerTextLegacy.text = timeString;
                UpdateColor(timerTextLegacy, remainingTime);
            }
        }
        
        void UpdateColor(Graphic text, float time)
        {
            if (time <= 10f)
                text.color = Color.red;
            else if (time <= 20f)
                text.color = Color.yellow;
            else
                text.color = Color.white;
        }
    }
}