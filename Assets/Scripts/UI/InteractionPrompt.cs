using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace EstiamGameJam2025
{
    public class InteractionPrompt : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject promptPanel;
        [SerializeField] private TextMeshProUGUI promptText;
        [SerializeField] private Image keyIcon;
        [SerializeField] private Sprite eKeySprite;
        
        [Header("Animation")]
        [SerializeField] private float fadeSpeed = 5f;
        [SerializeField] private float bobAmount = 10f;
        [SerializeField] private float bobSpeed = 2f;
        
        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;
        private Vector3 originalPosition;
        private bool isShowing = false;
        
        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            
            rectTransform = GetComponent<RectTransform>();
            originalPosition = rectTransform.anchoredPosition;
            
            // Cacher au départ
            canvasGroup.alpha = 0;
            gameObject.SetActive(false);
        }
        
        public void Show(string text = "Appuyer sur E")
        {
            isShowing = true;
            gameObject.SetActive(true);
            
            if (promptText != null)
                promptText.text = text;
                
            StopAllCoroutines();
            StartCoroutine(FadeIn());
        }
        
        public void Hide()
        {
            isShowing = false;
            StopAllCoroutines();
            StartCoroutine(FadeOut());
        }
        
        System.Collections.IEnumerator FadeIn()
        {
            while (canvasGroup.alpha < 1f)
            {
                canvasGroup.alpha += Time.deltaTime * fadeSpeed;
                yield return null;
            }
            canvasGroup.alpha = 1f;
        }
        
        System.Collections.IEnumerator FadeOut()
        {
            while (canvasGroup.alpha > 0f)
            {
                canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
                yield return null;
            }
            canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
        }
        
        void Update()
        {
            if (isShowing)
            {
                // Animation de flottement
                float yOffset = Mathf.Sin(Time.time * bobSpeed) * bobAmount;
                rectTransform.anchoredPosition = originalPosition + new Vector3(0, yOffset, 0);
            }
        }
    }
}