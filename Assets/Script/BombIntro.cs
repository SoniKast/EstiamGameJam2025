using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BombIntro : MonoBehaviour
{
    [Header("Bombe")]
    public Animator bombAnimator;                    // Animator de la bombe
    public string explosionAnim = "Explosion";       // Nom de l’animation d’explosion
    public string rewindAnim = "Explosion_Reverse";  // Nom de l’animation de rewind

    [Header("Timing")]
    public float pauseBeforeExplosion = 1.0f;
    public float explosionDuration = 1.2f;
    public float rewindDuration = 1.2f;
    public float pauseAfterReverse = 0.5f;

    [Header("Camera Shake")]
    public CameraShake cameraShake;       // Script de shake de caméra
    public float shakeDuration = 1.0f;
    public float shakeMagnitude = 0.4f;

    [Header("Glitch (UI Fullscreen)")]
    public GameObject glitchCanvas;       // Canvas contenant glitch + flèche
    public Animator glitchAnimator;       // Animator du glitch
    public Image glitchImage;             // Image UI du glitch
    public string glitchAnimation = "GlitchAnim";
    public float glitchAlpha = 0.5f;
    public float glitchFadeDuration = 0.3f;

    [Header("Flèche Rewind UI")]
    public Image rewindIcon;              // Image de la double flèche
    public float iconAlpha = 1f;
    public float iconFadeDuration = 0.3f;

    private bool hasStarted = false;

    void Start()
    {
        // Masquer les visuels au démarrage (si jamais l’objet est actif dans l’éditeur)
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
        // 1. Attente avant explosion
        yield return new WaitForSeconds(pauseBeforeExplosion);

        // 2. Explosion + caméra shake
        bombAnimator.Play(explosionAnim, 0, 0f); // Play depuis le début
        StartCoroutine(cameraShake.Shake(shakeDuration, shakeMagnitude));

        // Attendre la durée exacte de l’explosion avant de passer au rewind
        yield return new WaitForSeconds(explosionDuration);

        // 3. Glitch + flèche (fade in)
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

        // 4. Rewind animations
        glitchAnimator.Play(glitchAnimation, 0, 0f); // Jouer depuis 0
        bombAnimator.Play(rewindAnim, 0, 0f); // Jouer depuis 0

        yield return new WaitForSeconds(rewindDuration);

        // 5. Glitch + flèche (fade out)
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

        // 6. Pause après le rewind
        yield return new WaitForSeconds(pauseAfterReverse);

        // 7. Démarrage du niveau
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
        Debug.Log("🚀 Niveau lancé !");
        // Tu peux ici charger la scène ou activer le gameplay
        // Ex: SceneManager.LoadScene("NomDuNiveau");
    }
}
