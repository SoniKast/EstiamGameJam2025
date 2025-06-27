using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    public string nextSceneName = "SampleScene";

    [Header("Audio")]
    public AudioSource explosionAudio;
    public AudioSource rewindAudio;
    public AudioSource glitchAudio;

    private bool hasStarted = false;

    void Start()
    {
        // Cache les visuels glitch au démarrage
        if (glitchCanvas != null) glitchCanvas.SetActive(false);
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
        // 1. Pause avant explosion
        yield return new WaitForSeconds(pauseBeforeExplosion);

        // 2. Explosion visuelle + audio + shake
        if (bombAnimator != null) bombAnimator.Play(explosionAnim, 0, 0f);
        if (explosionAudio != null) explosionAudio.Play();
        if (cameraShake != null) StartCoroutine(cameraShake.Shake(shakeDuration, shakeMagnitude));
        yield return new WaitForSeconds(explosionDuration);

        // 3. Apparition glitch + icône rewind (fade in)
        if (glitchCanvas != null) glitchCanvas.SetActive(true);
        yield return FadeUI(glitchAlpha, iconAlpha, glitchFadeDuration);

        // 4. Lancer glitch anim + rewind
        if (glitchAudio != null) glitchAudio.Play();
        if (glitchAnimator != null) glitchAnimator.Play(glitchAnimation, 0, 0f);
        if (bombAnimator != null) bombAnimator.Play(rewindAnim, 0, 0f);
        if (rewindAudio != null) rewindAudio.Play();

        yield return new WaitForSeconds(rewindDuration);

        // 5. Disparition glitch + icône rewind (fade out)
        yield return FadeUI(0f, 0f, glitchFadeDuration);
        if (glitchCanvas != null) glitchCanvas.SetActive(false);

        // 6. Attente + chargement de scène
        yield return new WaitForSeconds(pauseAfterReverse);
        StartLevel();
    }

    IEnumerator FadeUI(float targetGlitchAlpha, float targetIconAlpha, float duration)
    {
        float startGlitchAlpha = glitchImage != null ? glitchImage.color.a : 0f;
        float startIconAlpha = rewindIcon != null ? rewindIcon.color.a : 0f;
        float t = 0f;

        while (t < duration)
        {
            float gA = Mathf.Lerp(startGlitchAlpha, targetGlitchAlpha, t / duration);
            float iA = Mathf.Lerp(startIconAlpha, targetIconAlpha, t / duration);

            SetImageAlpha(glitchImage, gA);
            SetImageAlpha(rewindIcon, iA);

            t += Time.deltaTime;
            yield return null;
        }

        SetImageAlpha(glitchImage, targetGlitchAlpha);
        SetImageAlpha(rewindIcon, targetIconAlpha);
    }

    void SetImageAlpha(Image img, float alpha)
    {
        if (img == null) return;
        Color c = img.color;
        c.a = alpha;
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
            Debug.LogWarning("❗ Aucun nom de scène défini dans 'nextSceneName'.");
        }
    }
}
