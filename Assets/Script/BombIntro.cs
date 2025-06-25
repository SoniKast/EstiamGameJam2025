using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombIntro : MonoBehaviour
{
    public Animator animator;
    public CameraShake cameraShake;
    public GameObject glitchOverlay;
    public float pauseBeforeExplosion = 1f;
    public float explosionDuration = 2f;
    public float reverseDuration = 2f;
    public float pauseAfterReverse = 1f;

    void Start()
    {
        StartCoroutine(PlayIntroSequence());
        glitchOverlay.SetActive(false);
    }
    IEnumerator PlayIntroSequence()
    {
        yield return new WaitForSeconds(pauseBeforeExplosion);

        animator.Play("Explosion");
        StartCoroutine(cameraShake.Shake(2.0f, 0.5f));
        yield return new WaitForSeconds(explosionDuration);

        glitchOverlay.SetActive(true);
        animator.Play("Explosion_Reverse");
        yield return new WaitForSeconds(reverseDuration);

        glitchOverlay.SetActive(false);

        yield return new WaitForSeconds(pauseAfterReverse);
        StartLevel();
    }

    void StartLevel()
    {
        Debug.Log("Niveau lancé !");
    }
}
