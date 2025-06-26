using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace EstiamGameJam2025
{
    public enum MiniGameType
    {
        SwitchActivation,    // Enigme1 : Activation des interrupteurs
        CableMatch,          // Enigme2 : Connexion des câbles  
        NumberSequence       // Enigme3 : Séquence numérique
    }

    [System.Serializable]
    public class MiniGame
    {
        public MiniGameType type;
        public string name;
        public string instructions;
        public GameObject miniGamePrefab;
        public float timeLimit = 30f;
        public int difficulty = 1;
    }

    public class MiniGameManager : MonoBehaviour
    {
        [Header("MiniGame Settings")]
        [SerializeField] private List<MiniGame> availableMiniGames = new List<MiniGame>();
        [SerializeField] private Canvas miniGameCanvas;
        [SerializeField] private GameObject miniGameContainer;
        
        [Header("UI References")]
        [SerializeField] private GameObject miniGameUI;
        [SerializeField] private TextMeshProUGUI instructionsText;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private Button closeButton;
        
        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip successSound;
        [SerializeField] private AudioClip failureSound;
        [SerializeField] private AudioClip tickSound;
        
        // État actuel
        private MiniGame currentMiniGame;
        private GameObject currentMiniGameInstance;
        private bool miniGameActive = false;
        private float miniGameTimer = 0f;
        private Coroutine timerCoroutine;
        
        // Events
        public delegate void OnMiniGameComplete(bool success);
        public event OnMiniGameComplete onMiniGameComplete;

        private void Start()
        {
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
            
            // Cacher l'UI au départ
            if (miniGameUI != null) miniGameUI.SetActive(false);
            
            // Configurer le bouton de fermeture
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(() => CompleteMiniGame(false));
            }
            
            // Initialiser les mini-jeux par défaut si la liste est vide
            if (availableMiniGames.Count == 0)
            {
                InitializeDefaultMiniGames();
            }
        }

        private void InitializeDefaultMiniGames()
        {
            // Switch Activation (Enigme1)
            availableMiniGames.Add(new MiniGame
            {
                type = MiniGameType.SwitchActivation,
                name = "Activation des interrupteurs",
                instructions = "Activez tous les interrupteurs !",
                timeLimit = 15f,
                difficulty = 1
            });
            
            // Cable Match (Enigme2)
            availableMiniGames.Add(new MiniGame
            {
                type = MiniGameType.CableMatch,
                name = "Connexion des câbles",
                instructions = "Connectez les câbles de même couleur !",
                timeLimit = 20f,
                difficulty = 1
            });
            
            // Number Sequence (Enigme3)
            availableMiniGames.Add(new MiniGame
            {
                type = MiniGameType.NumberSequence,
                name = "Séquence numérique",
                instructions = "Cliquez sur les numéros dans l'ordre (0-9) !",
                timeLimit = 25f,
                difficulty = 2
            });
        }

        public void StartMiniGame(MiniGameType type)
        {
            MiniGame miniGame = availableMiniGames.Find(mg => mg.type == type);
            if (miniGame != null)
            {
                StartSpecificMiniGame(miniGame);
            }
            else
            {
                Debug.LogWarning($"Mini-jeu {type} non trouvé !");
                StartRandomMiniGame();
            }
        }

        public void StartRandomMiniGame()
        {
            if (availableMiniGames.Count == 0)
            {
                Debug.LogError("Aucun mini-jeu disponible !");
                return;
            }
            
            MiniGame randomMiniGame = availableMiniGames[Random.Range(0, availableMiniGames.Count)];
            StartSpecificMiniGame(randomMiniGame);
        }

        private void StartSpecificMiniGame(MiniGame miniGame)
        {
            if (miniGameActive) return;
            
            currentMiniGame = miniGame;
            miniGameActive = true;
            miniGameTimer = miniGame.timeLimit;
            
            // Pause le temps principal
            TimeManager timeManager = FindObjectOfType<TimeManager>();
            if (timeManager != null)
            {
                timeManager.PauseTimer();
            }
            
            // Afficher l'UI
            ShowMiniGameUI();
            
            // Créer le mini-jeu
            CreateMiniGameInstance();
            
            // Démarrer le timer
            if (timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine);
            }
            timerCoroutine = StartCoroutine(Co_MiniGameTimer());
        }

        private void ShowMiniGameUI()
        {
            if (miniGameUI != null)
            {
                miniGameUI.SetActive(true);
            }
            
            if (instructionsText != null)
            {
                instructionsText.text = currentMiniGame.instructions;
            }
            
            UpdateTimerDisplay();
        }

        private void CreateMiniGameInstance()
        {
            // Nettoyer l'ancien mini-jeu
            if (currentMiniGameInstance != null)
            {
                Destroy(currentMiniGameInstance);
            }
            
            // Créer le nouveau mini-jeu
            if (currentMiniGame.miniGamePrefab != null)
            {
                currentMiniGameInstance = Instantiate(currentMiniGame.miniGamePrefab, miniGameContainer.transform);
            }
            else
            {
                // Créer un mini-jeu par défaut selon le type
                currentMiniGameInstance = CreateDefaultMiniGame(currentMiniGame.type);
            }
            
        }

        private GameObject CreateDefaultMiniGame(MiniGameType type)
        {
            GameObject miniGameObject = new GameObject($"MiniGame_{type}");
            miniGameObject.transform.SetParent(miniGameContainer.transform);
            
            switch (type)
            {
                case MiniGameType.SwitchActivation:
                    CreateSwitchActivationGame(miniGameObject);
                    break;
                case MiniGameType.CableMatch:
                    CreateCableMatchGame(miniGameObject);
                    break;
                case MiniGameType.NumberSequence:
                    CreateNumberSequenceGame(miniGameObject);
                    break;
                default:
                    CreateSimpleButtonGame(miniGameObject);
                    break;
            }
            
            return miniGameObject;
        }

private void CreateSwitchActivationGame(GameObject parent)
        {
            RectTransform rect = parent.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
            
            SwitchMiniGame switchGame = parent.AddComponent<SwitchMiniGame>();
            switchGame.Initialize(currentMiniGame.difficulty, CompleteMiniGame);
        }

        private void CreateCableMatchGame(GameObject parent)
        {
            RectTransform rect = parent.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
            
            CableMatchMiniGame cableGame = parent.AddComponent<CableMatchMiniGame>();
            cableGame.Initialize(currentMiniGame.difficulty, CompleteMiniGame);
        }

        private void CreateNumberSequenceGame(GameObject parent)
        {
            RectTransform rect = parent.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
            
            NumberSequenceMiniGame numberGame = parent.AddComponent<NumberSequenceMiniGame>();
            numberGame.Initialize(currentMiniGame.difficulty, CompleteMiniGame);
        }

        private void CreateSimpleButtonGame(GameObject parent)
        {
            // Créer un simple bouton pour tester
            GameObject buttonObj = new GameObject("TestButton");
            buttonObj.transform.SetParent(parent.transform);
            
            RectTransform rect = buttonObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(200, 50);
            rect.anchoredPosition = Vector2.zero;
            
            Image image = buttonObj.AddComponent<Image>();
            image.color = Color.green;
            
            Button button = buttonObj.AddComponent<Button>();
            button.onClick.AddListener(() => CompleteMiniGame(true));
            
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform);
            
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;
            
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = "RÉSOUDRE";
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.black;
        }


        private IEnumerator Co_MiniGameTimer()
        {
            while (miniGameTimer > 0 && miniGameActive)
            {
                miniGameTimer -= Time.unscaledDeltaTime;
                UpdateTimerDisplay();
                
                // Son de tic-tac dans les dernières secondes
                if (miniGameTimer <= 5f && tickSound != null)
                {
                    if (Mathf.FloorToInt(miniGameTimer) != Mathf.FloorToInt(miniGameTimer + Time.unscaledDeltaTime))
                    {
                        audioSource.PlayOneShot(tickSound);
                    }
                }
                
                yield return null;
            }
            
            if (miniGameActive)
            {
                // Temps écoulé - échec
                CompleteMiniGame(false);
            }
        }

        private void UpdateTimerDisplay()
        {
            if (timerText != null)
            {
                timerText.text = $"Temps: {Mathf.CeilToInt(miniGameTimer)}s";
                
                // Changer la couleur selon le temps restant
                if (miniGameTimer <= 5f)
                {
                    timerText.color = Color.red;
                }
                else if (miniGameTimer <= 10f)
                {
                    timerText.color = Color.yellow;
                }
                else
                {
                    timerText.color = Color.white;
                }
            }
        }

        public void CompleteMiniGame(bool success)
        {
            if (!miniGameActive) return;
            
            miniGameActive = false;
            
            // Arrêter le timer
            if (timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine);
            }
            
            // Jouer le son approprié
            if (audioSource != null)
            {
                audioSource.PlayOneShot(success ? successSound : failureSound);
            }
            
            // Nettoyer le mini-jeu
            if (currentMiniGameInstance != null)
            {
                Destroy(currentMiniGameInstance);
            }
            
            // Cacher l'UI
            if (miniGameUI != null)
            {
                miniGameUI.SetActive(false);
            }
            
            // Reprendre le timer principal
            TimeManager timeManager = FindObjectOfType<TimeManager>();
            if (timeManager != null)
            {
                timeManager.ResumeTimer();
            }
            
            // Notifier le GameManager
            onMiniGameComplete?.Invoke(success);
            GameManager.Instance?.CompleteMiniGame(success);
            
            // Si succès, prévenir la catastrophe
            if (success)
            {
                CatastropheManager catastropheManager = FindObjectOfType<CatastropheManager>();
                catastropheManager?.PreventCatastrophe();
            }
        }

        // Méthodes publiques pour les mini-jeux
        public void PlaySound(AudioClip clip)
        {
            if (audioSource != null && clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }

        public float GetTimeRemaining() => miniGameTimer;
        public bool IsMiniGameActive() => miniGameActive;
    }

}