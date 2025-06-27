using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EstiamGameJam2025
{
    public enum GameState
    {
        Menu,
        Playing,
        Rewinding,
        Investigating,
        MiniGame,
        GameOver,
        Victory
    }

    public class GameManager : MonoBehaviour
    {
        // Singleton
        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GameManager>();
                }
                return _instance;
            }
        }

        // État du jeu
        [Header("Game State")]
        [SerializeField] private GameState currentState = GameState.Menu;
        public GameState CurrentState => currentState;

        // Paramètres du jeu
        [Header("Game Settings")]
        [SerializeField] private float catastropheDelay = 5f; // Délai avant la catastrophe
        [SerializeField] private float rewindDuration = 3f; // Durée du rewind
        [SerializeField] private float investigationTime = 60f; // Temps pour enquêter

        // Références aux autres managers
        private TimeManager timeManager;
        private CatastropheManager catastropheManager;
        private MiniGameManager miniGameManager;

        // Variables de jeu
        private bool catastrophePrevented = false;
        private int currentLevel = 1;
        private float gameTime = 0f;

        // Events
        public delegate void OnGameStateChanged(GameState newState);
        public event OnGameStateChanged onGameStateChanged;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            InitializeManagers();
        }

        private void Update()
        {
            if (currentState == GameState.Playing || currentState == GameState.Investigating)
            {
                gameTime += Time.deltaTime;
            }
        }

        private void InitializeManagers()
        {
            // Récupération des références
            timeManager = FindObjectOfType<TimeManager>();
            catastropheManager = FindObjectOfType<CatastropheManager>();
            miniGameManager = FindObjectOfType<MiniGameManager>();
            
            // Créer les managers s'ils n'existent pas
            if (timeManager == null)
            {
                GameObject timeManagerObj = new GameObject("TimeManager");
                timeManager = timeManagerObj.AddComponent<TimeManager>();
            }
            
            if (catastropheManager == null)
            {
                GameObject catastropheManagerObj = new GameObject("CatastropheManager");
                catastropheManager = catastropheManagerObj.AddComponent<CatastropheManager>();
            }
            
            if (miniGameManager == null)
            {
                GameObject miniGameManagerObj = new GameObject("MiniGameManager");
                miniGameManager = miniGameManagerObj.AddComponent<MiniGameManager>();
            }
        }

        public void StartGame()
        {
            catastrophePrevented = false;
            gameTime = 0f;
            ChangeState(GameState.Playing);
            StartCoroutine(Co_GameSequence());
        }

        private IEnumerator Co_GameSequence()
        {
            // Phase 1: Jeu normal jusqu'à la catastrophe
            yield return new WaitForSeconds(catastropheDelay);

            // Déclencher la catastrophe
            TriggerCatastrophe();
            yield return new WaitForSeconds(2f); // Montrer la catastrophe

            // Phase 2: Rewind
            ChangeState(GameState.Rewinding);
            StartRewind();
            yield return new WaitForSeconds(rewindDuration);

            // Phase 3: Investigation
            ChangeState(GameState.Investigating);
            StartInvestigation();
        }

        private void TriggerCatastrophe()
        {
            Debug.Log("CATASTROPHE TRIGGERED!");
            catastropheManager?.PrepareCatastrophe();
            catastropheManager?.TriggerCatastrophe();
        }

        private void StartRewind()
        {
            Debug.Log("Starting rewind sequence...");
            timeManager?.StartRewind();
        }

        private void StartInvestigation()
        {
            Debug.Log("Investigation phase started! You have " + investigationTime + " seconds!");
            timeManager?.StartCountdown(investigationTime);
            catastropheManager?.ShowCatastropheTrigger();
        }

        public void ChangeState(GameState newState)
        {
            currentState = newState;
            Debug.Log("Game State changed to: " + newState);
            onGameStateChanged?.Invoke(newState);

            // Gérer les transitions d'état
            switch (newState)
            {
                case GameState.Menu:
                    Time.timeScale = 1f;
                    break;
                case GameState.Playing:
                    Time.timeScale = 1f;
                    break;
                case GameState.Rewinding:
                    // Le TimeManager gérera la vitesse
                    break;
                case GameState.Investigating:
                    Time.timeScale = 1f;
                    break;
                case GameState.MiniGame:
                    Time.timeScale = 0f; // Pause pendant les mini-jeux
                    break;
                case GameState.GameOver:
                    Time.timeScale = 0f;
                    OnGameOver();
                    break;
                case GameState.Victory:
                    Time.timeScale = 0f;
                    OnVictory();
                    break;
            }
        }

        public void StartMiniGame()
        {
            ChangeState(GameState.MiniGame);
            
            // Déterminer quel mini-jeu lancer selon la catastrophe
            if (catastropheManager != null && miniGameManager != null)
            {
                var catastrophe = catastropheManager.GetCurrentCatastrophe();
                if (catastrophe != null)
                {
                    // Mapper les types de catastrophe aux mini-jeux
                    switch (catastrophe.type)
                    {
                        case CatastropheType.Explosion:
                            miniGameManager.StartMiniGame(MiniGameType.NumberSequence);
                            break;
                        case CatastropheType.Fire:
                        case CatastropheType.GasLeak:
                            miniGameManager.StartMiniGame(MiniGameType.SwitchActivation);
                            break;
                        case CatastropheType.Flooding:
                        case CatastropheType.Collapse:
                            miniGameManager.StartMiniGame(MiniGameType.CableMatch);
                            break;
                        case CatastropheType.ElectricalFailure:
                            miniGameManager.StartMiniGame(MiniGameType.NumberSequence);
                            break;
                        default:
                            miniGameManager.StartRandomMiniGame();
                            break;
                    }
                }
                else
                {
                    miniGameManager.StartRandomMiniGame();
                }
            }
        }

        public void CompleteMiniGame(bool success)
        {
            if (success)
            {
                Debug.Log("Mini-game completed successfully!");
                catastrophePrevented = true;
                ChangeState(GameState.Victory);
            }
            else
            {
                Debug.Log("Mini-game failed!");
                ChangeState(GameState.Investigating);
            }
        }

        private void OnGameOver()
        {
            Debug.Log("GAME OVER - The catastrophe couldn't be prevented!");
            // Afficher l'UI de game over
            StartCoroutine(Co_RestartLevel());
        }

        private void OnVictory()
        {
            Debug.Log("VICTORY - Catastrophe prevented!");
            // Afficher l'UI de victoire
            StartCoroutine(Co_NextLevel());
        }

        private IEnumerator Co_RestartLevel()
        {
            yield return new WaitForSecondsRealtime(3f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private IEnumerator Co_NextLevel()
        {
            yield return new WaitForSecondsRealtime(3f);
            currentLevel++;
            // Charger le niveau suivant ou recommencer avec une nouvelle catastrophe
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void OnTimeExpired()
        {
            if (currentState == GameState.Investigating)
            {
                ChangeState(GameState.GameOver);
            }
        }

        // Méthodes utilitaires
        public bool IsGameActive()
        {
            return currentState == GameState.Playing || 
                   currentState == GameState.Investigating || 
                   currentState == GameState.Rewinding;
        }

        public float GetGameTime() => gameTime;
        public bool IsCatastrophePrevented() => catastrophePrevented;
        public int GetCurrentLevel() => currentLevel;
    }
}