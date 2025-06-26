using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace EstiamGameJam2025
{
    public class NumberSequenceMiniGame : MonoBehaviour, IMiniGame
    {
        [Header("Configuration")]
        public Button[] numberButtons; // Boutons 0-9 dans l'ordre
        public TextMeshProUGUI instructionText;
        public GameObject errorFlashPanel;
        public Image[] progressIndicators; // Indicateurs visuels de progression
        
        [Header("Difficulty Settings")]
        [SerializeField] private bool[] shuffleByDifficulty = { false, true, true };
        [SerializeField] private float[] timeLimitByDifficulty = { 0f, 30f, 20f }; // 0 = pas de limite
        
        private int currentIndex = 0;
        private bool isCompleted = false;
        private int currentDifficulty = 1;
        private System.Action<bool> onCompleteCallback;
        private float timeRemaining;
        private bool hasTimeLimit;
        private Coroutine timerCoroutine;

        public void Initialize(int difficulty, System.Action<bool> onComplete)
        {
            currentDifficulty = Mathf.Clamp(difficulty, 1, 3);
            onCompleteCallback = onComplete;
            isCompleted = false;
            currentIndex = 0;
            
            hasTimeLimit = timeLimitByDifficulty[currentDifficulty - 1] > 0;
            timeRemaining = timeLimitByDifficulty[currentDifficulty - 1];

            if (instructionText)
            {
                string message = "Cliquez sur les numéros dans l'ordre (0-9)";
                if (hasTimeLimit)
                    message += $"\nTemps: {timeRemaining:F0}s";
                instructionText.text = message;
            }

            SetupButtons();
            
            if (hasTimeLimit)
            {
                timerCoroutine = StartCoroutine(TimerCountdown());
            }
        }

        private void SetupButtons()
        {
            // Active tous les boutons
            for (int i = 0; i < numberButtons.Length; i++)
            {
                numberButtons[i].gameObject.SetActive(true);
                numberButtons[i].interactable = true;
                
                // Configure le texte du bouton
                TextMeshProUGUI buttonText = numberButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText)
                    buttonText.text = i.ToString();
                
                // Ajoute le listener
                int buttonValue = i;
                numberButtons[i].onClick.RemoveAllListeners();
                numberButtons[i].onClick.AddListener(() => OnNumberClicked(buttonValue));
            }
            
            // Mélange les positions si nécessaire
            if (shuffleByDifficulty[currentDifficulty - 1])
            {
                ShuffleButtonPositions();
            }
            
            // Reset les indicateurs de progression
            UpdateProgressIndicators();
        }

        private void ShuffleButtonPositions()
        {
            List<Vector3> positions = new List<Vector3>();
            
            // Sauvegarde toutes les positions
            foreach (Button btn in numberButtons)
            {
                positions.Add(btn.transform.position);
            }
            
            // Mélange les positions
            for (int i = 0; i < positions.Count; i++)
            {
                int randomIndex = Random.Range(i, positions.Count);
                Vector3 temp = positions[i];
                positions[i] = positions[randomIndex];
                positions[randomIndex] = temp;
            }
            
            // Applique les nouvelles positions
            for (int i = 0; i < numberButtons.Length; i++)
            {
                numberButtons[i].transform.position = positions[i];
            }
        }

        private void OnNumberClicked(int number)
        {
            if (isCompleted) return;

            if (number == currentIndex)
            {
                // Bon numéro
                numberButtons[number].interactable = false;
                
                // Change la couleur du bouton pour indiquer qu'il est validé
                Image btnImage = numberButtons[number].GetComponent<Image>();
                if (btnImage)
                    btnImage.color = Color.green;
                
                currentIndex++;
                UpdateProgressIndicators();
                
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySound("Correct");

                // Vérifie si la séquence est complète
                if (currentIndex >= numberButtons.Length)
                {
                    CompleteMiniGame(true);
                }
            }
            else
            {
                // Mauvais numéro
                StartCoroutine(ShowError());
                ResetSequence();
                
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySound("Error");
            }
        }

        private void UpdateProgressIndicators()
        {
            if (progressIndicators == null) return;
            
            for (int i = 0; i < progressIndicators.Length && i < numberButtons.Length; i++)
            {
                if (i < currentIndex)
                {
                    progressIndicators[i].color = Color.green;
                }
                else
                {
                    progressIndicators[i].color = Color.gray;
                }
            }
        }

        private IEnumerator TimerCountdown()
        {
            while (timeRemaining > 0 && !isCompleted)
            {
                timeRemaining -= Time.deltaTime;
                
                if (instructionText)
                {
                    string message = "Cliquez sur les numéros dans l'ordre (0-9)";
                    message += $"\nTemps: {timeRemaining:F1}s";
                    
                    // Change la couleur si peu de temps reste
                    if (timeRemaining < 10)
                        instructionText.color = Color.red;
                    else if (timeRemaining < 20)
                        instructionText.color = Color.yellow;
                    
                    instructionText.text = message;
                }
                
                yield return null;
            }
            
            if (!isCompleted && timeRemaining <= 0)
            {
                // Temps écoulé
                CompleteMiniGame(false);
            }
        }

        private void ResetSequence()
        {
            currentIndex = 0;
            
            // Réactive tous les boutons
            foreach (Button btn in numberButtons)
            {
                btn.interactable = true;
                Image btnImage = btn.GetComponent<Image>();
                if (btnImage)
                    btnImage.color = Color.white;
            }
            
            UpdateProgressIndicators();
        }

        private IEnumerator ShowError()
        {
            if (errorFlashPanel)
            {
                errorFlashPanel.SetActive(true);
                yield return new WaitForSeconds(0.5f);
                errorFlashPanel.SetActive(false);
            }
        }

        private void CompleteMiniGame(bool success)
        {
            if (isCompleted) return;
            
            isCompleted = true;
            
            if (timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine);
            }
            
            if (instructionText)
            {
                instructionText.color = Color.white;
                instructionText.text = success ? "Séquence complète!" : "Temps écoulé!";
            }
            
            StartCoroutine(CompleteAfterDelay(success));
        }

        private IEnumerator CompleteAfterDelay(bool success)
        {
            yield return new WaitForSeconds(1f);
            onCompleteCallback?.Invoke(success);
        }

        public void Reset()
        {
            isCompleted = false;
            currentIndex = 0;
            
            if (timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine);
            }
            
            timeRemaining = timeLimitByDifficulty[currentDifficulty - 1];
            
            if (instructionText)
            {
                instructionText.color = Color.white;
                string message = "Cliquez sur les numéros dans l'ordre (0-9)";
                if (hasTimeLimit)
                    message += $"\nTemps: {timeRemaining:F0}s";
                instructionText.text = message;
            }
            
            SetupButtons();
            
            if (hasTimeLimit)
            {
                timerCoroutine = StartCoroutine(TimerCountdown());
            }
        }

        public void ForceComplete()
        {
            CompleteMiniGame(false);
        }

        void OnDestroy()
        {
            // Nettoie les listeners
            foreach (Button btn in numberButtons)
            {
                if (btn != null)
                    btn.onClick.RemoveAllListeners();
            }
            
            if (timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine);
            }
        }
    }
}