using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace EstiamGameJam2025
{
    public class SwitchMiniGame : MonoBehaviour, IMiniGame
    {
        [System.Serializable]
        public class Switch
        {
            public Toggle toggle;
            public Image diodeImage;
            public Color onColor = Color.green;
            public Color offColor = Color.red;
        }

        [Header("Configuration")]
        public Switch[] switches;
        public TextMeshProUGUI instructionText;
        
        [Header("Difficulty Settings")]
        [SerializeField] private int[] switchCountByDifficulty = { 3, 4, 5 };
        
        private bool isCompleted = false;
        private int currentDifficulty = 1;
        private System.Action<bool> onCompleteCallback;

        public void Initialize(int difficulty, System.Action<bool> onComplete)
        {
            currentDifficulty = Mathf.Clamp(difficulty, 1, 3);
            onCompleteCallback = onComplete;
            isCompleted = false;

            if (instructionText)
                instructionText.text = "Activez tous les interrupteurs!";

            SetupSwitches();
        }

        private void SetupSwitches()
        {
            int switchCount = switchCountByDifficulty[currentDifficulty - 1];
            
            // Active le bon nombre d'interrupteurs selon la difficulté
            for (int i = 0; i < switches.Length; i++)
            {
                if (i < switchCount)
                {
                    switches[i].toggle.gameObject.SetActive(true);
                    switches[i].toggle.isOn = false;
                    switches[i].diodeImage.color = switches[i].offColor;
                    
                    // Ajoute le listener pour mettre à jour la diode
                    int index = i;
                    switches[i].toggle.onValueChanged.RemoveAllListeners();
                    switches[i].toggle.onValueChanged.AddListener((value) => OnSwitchChanged(index, value));
                }
                else
                {
                    switches[i].toggle.gameObject.SetActive(false);
                }
            }
        }

        private void OnSwitchChanged(int index, bool value)
        {
            // Met à jour la couleur de la diode
            switches[index].diodeImage.color = value ? switches[index].onColor : switches[index].offColor;
            
            // Joue un son de clic
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySound("Click");
            
            // Vérifie si tous les interrupteurs sont activés
            if (CheckAllSwitchesOn())
            {
                CompleteMiniGame(true);
            }
        }

        private bool CheckAllSwitchesOn()
        {
            int switchCount = switchCountByDifficulty[currentDifficulty - 1];
            
            for (int i = 0; i < switchCount; i++)
            {
                if (!switches[i].toggle.isOn)
                    return false;
            }
            return true;
        }

        private void CompleteMiniGame(bool success)
        {
            if (isCompleted) return;
            
            isCompleted = true;
            
            if (success)
            {
                if (instructionText)
                    instructionText.text = "Tous les interrupteurs sont activés!";
                
                StartCoroutine(CompleteAfterDelay(success));
            }
        }

        private IEnumerator CompleteAfterDelay(bool success)
        {
            yield return new WaitForSeconds(1f);
            onCompleteCallback?.Invoke(success);
        }

        public void Reset()
        {
            isCompleted = false;
            SetupSwitches();
            
            if (instructionText)
                instructionText.text = "Activez tous les interrupteurs!";
        }

        public void ForceComplete()
        {
            CompleteMiniGame(false);
        }

        void OnDestroy()
        {
            // Nettoie les listeners
            foreach (var switchItem in switches)
            {
                if (switchItem.toggle != null)
                    switchItem.toggle.onValueChanged.RemoveAllListeners();
            }
        }
    }
}