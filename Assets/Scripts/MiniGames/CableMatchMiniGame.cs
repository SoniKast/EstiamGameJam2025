using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace EstiamGameJam2025
{
    public class CableMatchMiniGame : MonoBehaviour, IMiniGame
    {
        [System.Serializable]
        public class CablePoint
        {
            public string colorId;
            public Button startButton;
            public Button endButton;
            public Image connectionLine;
            public Color cableColor;
            public bool isConnected = false;
        }

        [Header("Configuration")]
        public CablePoint[] cablePoints;
        public TextMeshProUGUI instructionText;
        public GameObject errorFlashPanel;
        
        [Header("Difficulty Settings")]
        [SerializeField] private int[] cableCountByDifficulty = { 3, 4, 5 };
        
        private string selectedStartId = null;
        private bool isCompleted = false;
        private int currentDifficulty = 1;
        private System.Action<bool> onCompleteCallback;

        public void Initialize(int difficulty, System.Action<bool> onComplete)
        {
            currentDifficulty = Mathf.Clamp(difficulty, 1, 3);
            onCompleteCallback = onComplete;
            isCompleted = false;
            selectedStartId = null;

            if (instructionText)
                instructionText.text = "Connectez les câbles de même couleur!";

            SetupCables();
        }

        private void SetupCables()
        {
            int cableCount = cableCountByDifficulty[currentDifficulty - 1];
            
            // Configure les câbles selon la difficulté
            for (int i = 0; i < cablePoints.Length; i++)
            {
                if (i < cableCount)
                {
                    cablePoints[i].startButton.gameObject.SetActive(true);
                    cablePoints[i].endButton.gameObject.SetActive(true);
                    cablePoints[i].connectionLine.gameObject.SetActive(false);
                    cablePoints[i].isConnected = false;
                    
                    // Configure les couleurs
                    Image startImage = cablePoints[i].startButton.GetComponent<Image>();
                    Image endImage = cablePoints[i].endButton.GetComponent<Image>();
                    if (startImage) startImage.color = cablePoints[i].cableColor;
                    if (endImage) endImage.color = cablePoints[i].cableColor;
                    
                    // Configure les boutons
                    cablePoints[i].startButton.interactable = true;
                    cablePoints[i].endButton.interactable = true;
                    
                    // Ajoute les listeners
                    string colorId = cablePoints[i].colorId;
                    cablePoints[i].startButton.onClick.RemoveAllListeners();
                    cablePoints[i].endButton.onClick.RemoveAllListeners();
                    cablePoints[i].startButton.onClick.AddListener(() => OnStartSelected(colorId));
                    cablePoints[i].endButton.onClick.AddListener(() => OnEndSelected(colorId));
                }
                else
                {
                    cablePoints[i].startButton.gameObject.SetActive(false);
                    cablePoints[i].endButton.gameObject.SetActive(false);
                    cablePoints[i].connectionLine.gameObject.SetActive(false);
                }
            }
            
            // Mélange les positions des boutons de fin
            ShuffleEndButtons(cableCount);
        }

        private void ShuffleEndButtons(int count)
        {
            List<Vector3> positions = new List<Vector3>();
            
            // Sauvegarde les positions
            for (int i = 0; i < count; i++)
            {
                positions.Add(cablePoints[i].endButton.transform.position);
            }
            
            // Mélange
            for (int i = 0; i < positions.Count; i++)
            {
                int randomIndex = Random.Range(i, positions.Count);
                Vector3 temp = positions[i];
                positions[i] = positions[randomIndex];
                positions[randomIndex] = temp;
            }
            
            // Applique les nouvelles positions
            for (int i = 0; i < count; i++)
            {
                cablePoints[i].endButton.transform.position = positions[i];
            }
        }

        private void OnStartSelected(string colorId)
        {
            selectedStartId = colorId;
            
            // Feedback visuel
            foreach (var cable in cablePoints)
            {
                if (cable.colorId == colorId && cable.startButton.gameObject.activeSelf)
                {
                    cable.startButton.GetComponent<Image>().color = cable.cableColor * 1.5f;
                    break;
                }
            }
            
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySound("Select");
        }

        private void OnEndSelected(string colorId)
        {
            if (string.IsNullOrEmpty(selectedStartId)) return;

            if (colorId == selectedStartId)
            {
                // Connexion réussie
                ConnectCable(colorId);
                
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySound("Connect");
                
                CheckVictory();
            }
            else
            {
                // Mauvaise connexion
                StartCoroutine(ShowError());
                ResetAll();
                
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySound("Error");
            }

            selectedStartId = null;
        }

        private void ConnectCable(string colorId)
        {
            foreach (var cable in cablePoints)
            {
                if (cable.colorId == colorId)
                {
                    cable.isConnected = true;
                    cable.startButton.interactable = false;
                    cable.endButton.interactable = false;
                    cable.connectionLine.gameObject.SetActive(true);
                    
                    // Positionne la ligne de connexion
                    PositionConnectionLine(cable);
                    break;
                }
            }
        }

        private void PositionConnectionLine(CablePoint cable)
        {
            Vector3 startPos = cable.startButton.transform.position;
            Vector3 endPos = cable.endButton.transform.position;
            
            cable.connectionLine.transform.position = (startPos + endPos) / 2;
            
            Vector3 direction = endPos - startPos;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            cable.connectionLine.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            
            float distance = Vector3.Distance(startPos, endPos);
            cable.connectionLine.rectTransform.sizeDelta = new Vector2(distance, 5);
        }

        private void CheckVictory()
        {
            int cableCount = cableCountByDifficulty[currentDifficulty - 1];
            
            for (int i = 0; i < cableCount; i++)
            {
                if (!cablePoints[i].isConnected)
                    return;
            }

            CompleteMiniGame(true);
        }

        private void CompleteMiniGame(bool success)
        {
            if (isCompleted) return;
            
            isCompleted = true;
            
            if (instructionText)
                instructionText.text = "Tous les câbles sont connectés!";
            
            StartCoroutine(CompleteAfterDelay(success));
        }

        private IEnumerator CompleteAfterDelay(bool success)
        {
            yield return new WaitForSeconds(1f);
            onCompleteCallback?.Invoke(success);
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

        private void ResetAll()
        {
            selectedStartId = null;
            
            int cableCount = cableCountByDifficulty[currentDifficulty - 1];
            
            for (int i = 0; i < cableCount; i++)
            {
                cablePoints[i].isConnected = false;
                cablePoints[i].startButton.interactable = true;
                cablePoints[i].endButton.interactable = true;
                cablePoints[i].connectionLine.gameObject.SetActive(false);
                
                // Reset couleur
                Image startImage = cablePoints[i].startButton.GetComponent<Image>();
                if (startImage) startImage.color = cablePoints[i].cableColor;
            }
        }

        public void Reset()
        {
            isCompleted = false;
            selectedStartId = null;
            SetupCables();
            
            if (instructionText)
                instructionText.text = "Connectez les câbles de même couleur!";
        }

        public void ForceComplete()
        {
            CompleteMiniGame(false);
        }

        void OnDestroy()
        {
            // Nettoie les listeners
            foreach (var cable in cablePoints)
            {
                if (cable.startButton != null)
                    cable.startButton.onClick.RemoveAllListeners();
                if (cable.endButton != null)
                    cable.endButton.onClick.RemoveAllListeners();
            }
        }
    }
}