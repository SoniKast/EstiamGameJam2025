using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // 🔁 Nécessaire pour charger les scènes

public class BombIntro : MonoBehaviour
{
    [Header("Bombe")]
    public Animator bombAnimator;
    public string explosionAnim = "Explosion";
    public string rewindAnim = "Explosion_Reverse";

    [Header("Timing")]
    public float pauseBeforeExplosion = 1.0f;
    public float explosionDuration = 1.2f;
    public float rewindDuration = 1.2f;
    public float pauseAfterReverse = 0.5f;

    [Header("Camera Shake")]
    public CameraShake cameraShake;
    public float shakeDuration = 1.0f;
    public float shakeMagnitude = 0.4f;

    [Header("Glitch (UI Fullscreen)")]
    public GameObject glitchCanvas;
    public Animator glitchAnimator;
    public Image glitchImage;
    public string glitchAnimation = "GlitchAnim";
    public float glitchAlpha = 0.5f;
    public float glitchFadeDuration = 0.3f;

    [Header("Flèche Rewind UI")]
    public Image rewindIcon;
    public float iconAlpha = 1f;
    public float iconFadeDuration = 0.3f;

    [Header("Scène à charger")]
    [Tooltip("Nom exact de la scène à charger après l'animation")]
    public string nextSceneName = "SampleScene"; // Par défaut, charge SampleScene

    private bool hasStarted = false;

    void Start()
    {
        // Cache UI au démarrage
        glitchCanvas.SetActive(false);
        SetImageAlpha(glitchImage, 0f);
        SetImageAlpha(rewindIcon, 0f);
    }

    public void TriggerExplosionSequence()
    {
        if (!hasStarted)
        {
            hasStarted = true;
            StartCoroutine(PlayIntroSequence());
        }
    }

    IEnumerator PlayIntroSequence()
    {
        yield return new WaitForSeconds(pauseBeforeExplosion);

        bombAnimator.Play(explosionAnim, 0, 0f);
        StartCoroutine(cameraShake.Shake(shakeDuration, shakeMagnitude));
        yield return new WaitForSeconds(explosionDuration);

        glitchCanvas.SetActive(true);
        float t = 0f;

        while (t < glitchFadeDuration)
        {
            float glitchA = Mathf.Lerp(0f, glitchAlpha, t / glitchFadeDuration);
            float iconA = Mathf.Lerp(0f, iconAlpha, t / iconFadeDuration);

            SetImageAlpha(glitchImage, glitchA);
            SetImageAlpha(rewindIcon, iconA);

            t += Time.deltaTime;
            yield return null;
        }

        SetImageAlpha(glitchImage, glitchAlpha);
        SetImageAlpha(rewindIcon, iconAlpha);

        glitchAnimator.Play(glitchAnimation, 0, 0f);
        bombAnimator.Play(rewindAnim, 0, 0f);

        yield return new WaitForSeconds(rewindDuration);

        t = 0f;
        while (t < glitchFadeDuration)
        {
            float glitchA = Mathf.Lerp(glitchAlpha, 0f, t / glitchFadeDuration);
            float iconA = Mathf.Lerp(iconAlpha, 0f, t / iconFadeDuration);

            SetImageAlpha(glitchImage, glitchA);
            SetImageAlpha(rewindIcon, iconA);

            t += Time.deltaTime;
            yield return null;
        }

        SetImageAlpha(glitchImage, 0f);
        SetImageAlpha(rewindIcon, 0f);
        glitchCanvas.SetActive(false);

        yield return new WaitForSeconds(pauseAfterReverse);

        StartLevel();
    }

    void SetImageAlpha(Image img, float a)
    {
        if (img == null) return;
        Color c = img.color;
        c.a = a;
        img.color = c;
    }

    void StartLevel()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            Debug.Log($"🚀 Chargement de la scène : {nextSceneName}");
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("❗ Aucun nom de scène spécifié dans 'nextSceneName'.");
        }
    }
}
