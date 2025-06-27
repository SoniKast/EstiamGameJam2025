using System.Collections;
using UnityEngine;

public class CharacterIntro : MonoBehaviour
{
    [Header("Déplacements")]
    public Transform placePoint;               // Point où il pose la bombe
    public Transform exitPoint;                // Point de sortie
    public float moveSpeed = 2f;

    [Header("Animations")]
    public Animator animator;                  // Animator du personnage
    public string walkInAnim = "WalkIn";
    public string placeBombAnim = "PlaceBomb";
    public string walkOutAnim = "WalkOut";

    [Header("Bombe")]
    public GameObject bombObject;              // GameObject de la bombe (désactivé au départ)

    [Header("Lien avec BombIntro")]
    public BombIntro bombIntro;                // Script BombIntro (appelé après la sortie)

    [Header("Audio")]
    public AudioSource walkAudio;              // Bruit de pas
    public AudioSource placeBombAudio;         // Son de pose de bombe

    void Start()
    {
        if (bombObject != null)
            bombObject.SetActive(false);       // Cache la bombe au début

        StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro()
    {
        // 1. Entrée du personnage
        animator.Play(walkInAnim);
        if (walkAudio != null) walkAudio.Play();

        yield return MoveTo(placePoint.position);

        if (walkAudio != null) walkAudio.Stop();

        // 2. Pose de la bombe
        animator.Play(placeBombAnim);

        yield return new WaitForSeconds(0.4f); // Laisse le temps à l'anim de commencer

        // 3. Active la bombe
        if (bombObject != null)
            bombObject.SetActive(true);

        yield return new WaitForSeconds(0.6f); // Fin de l'anim de pose

        // 4. Sortie du personnage
        animator.Play(walkOutAnim);
        if (walkAudio != null) walkAudio.Play(); // Rejoue le son de marche

        yield return MoveTo(exitPoint.position);

        if (walkAudio != null) walkAudio.Stop();

        // 5. Attente que le personnage sorte de l’écran
        yield return WaitUntilOffscreen();

        // 6. Déclenchement de l’explosion
        if (bombIntro != null)
            bombIntro.TriggerExplosionSequence();
    }

    IEnumerator MoveTo(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator WaitUntilOffscreen()
    {
        SpriteRenderer rend = GetComponentInChildren<SpriteRenderer>();
        if (rend == null)
        {
            Debug.LogWarning("Aucun SpriteRenderer trouvé.");
            yield break;
        }

        while (rend.isVisible)
        {
            yield return null;
        }
    }
}
