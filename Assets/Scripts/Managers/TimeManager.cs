using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace EstiamGameJam2025
{
    public class TimeManager : MonoBehaviour
    {
        [Header("Timer Settings")]
        [SerializeField] private float investigationTime = 60f;
        [SerializeField] private float rewindSpeed = 2f;
        
        [Header("UI References")]
        [SerializeField] private GameObject timerUI;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private Image timerFillImage;
        [SerializeField] private GameObject rewindEffectUI;
        
        [Header("Visual Effects")]
        [SerializeField] private Color normalTimerColor = Color.white;
        [SerializeField] private Color warningTimerColor = Color.yellow;
        [SerializeField] private Color criticalTimerColor = Color.red;
        [SerializeField] private float warningThreshold = 20f;
        [SerializeField] private float criticalThreshold = 10f;

        // Timer
        private float currentTime;
        private bool isCountingDown = false;
        private bool isRewinding = false;
        
        // Rewind
        private List<WorldState> recordedStates = new List<WorldState>();
        private int rewindIndex = 0;
        
        // Events
        public delegate void OnTimerExpired();
        public event OnTimerExpired onTimerExpired;
        
        public delegate void OnRewindComplete();
        public event OnRewindComplete onRewindComplete;

        // Structure pour enregistrer l'état du monde
        [System.Serializable]
        private class WorldState
        {
            public float timestamp;
            public Dictionary<int, ObjectState> objects = new Dictionary<int, ObjectState>();
        }
        
        [System.Serializable]
        private class ObjectState
        {
            public Vector3 position;
            public Quaternion rotation;
            public bool isActive;
        }

        private void Start()
        {
            // Cacher l'UI au départ
            if (timerUI != null) timerUI.SetActive(true);
            if (rewindEffectUI != null) rewindEffectUI.SetActive(false);
            
            // S'abonner aux changements d'état
            if (GameManager.Instance != null)
            {
                GameManager.Instance.onGameStateChanged += OnGameStateChanged;
            }
        }

        private void Update()
        {
            // Enregistrer l'état du monde pendant la phase de jeu
            if (GameManager.Instance != null && 
                GameManager.Instance.CurrentState == GameState.Playing)
            {
                RecordWorldState();
            }
            
            // Debug - voir pourquoi isCountingDown devient false
            if (!isCountingDown && currentTime > 0 && GameManager.Instance != null)
            {
                if (GameManager.Instance.CurrentState == GameState.Investigating && Time.frameCount % 60 == 0)
                {
                    Debug.LogWarning($"[TimeManager] Timer arrêté mais devrait tourner! État: {GameManager.Instance.CurrentState}, Temps: {currentTime:F1}");
                }
            }
            
            // Gérer le compte à rebours
            if (isCountingDown)
            {
                UpdateCountdown();
            }
        }

        private void RecordWorldState()
        {
            // Limiter le nombre d'états enregistrés (environ 10 secondes à 30 FPS)
            const int maxStates = 300;
            
            if (recordedStates.Count >= maxStates)
            {
                recordedStates.RemoveAt(0);
            }
            
            WorldState state = new WorldState();
            state.timestamp = Time.time;
            
            // Enregistrer tous les objets avec le tag "Recordable"
            GameObject[] recordableObjects = GameObject.FindGameObjectsWithTag("Recordable");
            
            foreach (GameObject obj in recordableObjects)
            {
                int id = obj.GetInstanceID();
                ObjectState objState = new ObjectState
                {
                    position = obj.transform.position,
                    rotation = obj.transform.rotation,
                    isActive = obj.activeSelf
                };
                
                state.objects[id] = objState;
            }
            
            recordedStates.Add(state);
        }

        public void StartCountdown(float duration)
        {
            currentTime = duration;
            investigationTime = duration;
            isCountingDown = true;
            
            if (timerUI != null) timerUI.SetActive(true);
            
            UpdateTimerDisplay();
        }

        public void StopCountdown()
        {
            isCountingDown = false;
        }

        private void UpdateCountdown()
        {
            // Debug pour comprendre pourquoi le timer s'arrête
            if (Time.deltaTime == 0)
            {
                Debug.LogWarning($"[TimeManager] Time.deltaTime = 0 ! TimeScale = {Time.timeScale}");
            }
            currentTime -= Time.deltaTime;
            
            if (currentTime <= 0)
            {
                currentTime = 0;
                isCountingDown = false;
                OnTimeExpired();
            }
            
            UpdateTimerDisplay();
        }

        private void UpdateTimerDisplay()
        {
            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(currentTime / 60f);
                int seconds = Mathf.FloorToInt(currentTime % 60f);
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
                // Changer la couleur selon le temps restant
                if (currentTime <= criticalThreshold)
                {
                    timerText.color = criticalTimerColor;
                }
                else if (currentTime <= warningThreshold)
                {
                    timerText.color = warningTimerColor;
                }
                else
                {
                    timerText.color = normalTimerColor;
                }
            }
            
            // Mettre à jour la barre de progression
            if (timerFillImage != null)
            {
                timerFillImage.fillAmount = currentTime / investigationTime;
            }
        }

        public void StartRewind()
        {
            if (recordedStates.Count == 0) return;
            
            isRewinding = true;
            rewindIndex = recordedStates.Count - 1;
            
            // Activer l'effet visuel de rewind
            if (rewindEffectUI != null) rewindEffectUI.SetActive(true);
            
            // Désactiver le timer pendant le rewind
            if (timerUI != null) timerUI.SetActive(false);
            
            // Démarrer le rewind du player
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.StartRewind();
            }
            
            StartCoroutine(Co_RewindSequence());
        }

        private IEnumerator Co_RewindSequence()
        {
            float rewindDuration = GameManager.Instance != null ? 3f : 3f; // Durée du rewind
            float elapsedTime = 0f;
            
            while (elapsedTime < rewindDuration && rewindIndex >= 0)
            {
                // Appliquer l'état enregistré
                ApplyWorldState(recordedStates[rewindIndex]);
                
                // Calculer le prochain index
                int framesToSkip = Mathf.CeilToInt(rewindSpeed);
                rewindIndex -= framesToSkip;
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            // Fin du rewind
            isRewinding = false;
            if (rewindEffectUI != null) rewindEffectUI.SetActive(false);
            
            onRewindComplete?.Invoke();
            
            // Commencer le compte à rebours après le rewind
            if (GameManager.Instance != null && 
                GameManager.Instance.CurrentState == GameState.Investigating)
            {
                StartCountdown(investigationTime);
            }
        }

        private void ApplyWorldState(WorldState state)
        {
            GameObject[] recordableObjects = GameObject.FindGameObjectsWithTag("Recordable");
            
            foreach (GameObject obj in recordableObjects)
            {
                int id = obj.GetInstanceID();
                
                if (state.objects.ContainsKey(id))
                {
                    ObjectState objState = state.objects[id];
                    obj.transform.position = objState.position;
                    obj.transform.rotation = objState.rotation;
                    obj.SetActive(objState.isActive);
                }
            }
        }

        private void OnTimeExpired()
        {
            Debug.Log("Time's up!");
            onTimerExpired?.Invoke();
            
            // Notifier le GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnTimeExpired();
            }
        }

        private void OnGameStateChanged(GameState newState)
        {
            Debug.Log($"[TimeManager] État du jeu changé : {newState}");
            switch (newState)
            {
                case GameState.Playing:
                    // Commencer à enregistrer
                    recordedStates.Clear();
                    break;
                    
                case GameState.Rewinding:
                    StartRewind();
                    break;
                    
                case GameState.Investigating:
                    // Le timer sera démarré après le rewind
                    break;
                    
                case GameState.GameOver:
                case GameState.Victory:
                    StopCountdown();
                    if (timerUI != null) timerUI.SetActive(false);
                    break;
            }
        }

        // Méthodes publiques
        public float GetRemainingTime() => currentTime;
        public bool IsCountingDown() => isCountingDown;
        public bool IsRewinding() => isRewinding;
        
        public void AddTime(float seconds)
        {
            currentTime = Mathf.Clamp(currentTime + seconds, 0f, investigationTime);
            UpdateTimerDisplay();
        }
        
        public void PauseTimer()
        {
            Debug.LogWarning($"[TimeManager] PauseTimer appelé! Temps actuel: {currentTime:F1}s");
            Debug.LogWarning($"[TimeManager] Appelé par: {System.Environment.StackTrace}");
            isCountingDown = false;
        }
        
        public void ResumeTimer()
        {
            if (currentTime > 0 && GameManager.Instance.CurrentState == GameState.Investigating)
            {
                isCountingDown = true;
            }
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