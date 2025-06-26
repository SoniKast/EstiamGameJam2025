using System.Collections;
using UnityEngine;

public class CharacterIntro : MonoBehaviour
{
    [Header("Déplacements")]
    public Transform placePoint;              // Point où il pose la bombe
    public Transform exitPoint;               // Point de sortie
    public float moveSpeed = 2f;

    [Header("Animation")]
    public Animator animator;                 // Animator du personnage
    public string walkInAnim = "WalkIn";
    public string placeBombAnim = "PlaceBomb";
    public string walkOutAnim = "WalkOut";

    [Header("Bombe")]
    public GameObject bombObject;             // GameObject de la bombe à activer

    [Header("Lien avec BombIntro")]
    public BombIntro bombIntro;               // Script BombIntro à appeler après la sortie

    void Start()
    {
        // Désactive la bombe au démarrage
        if (bombObject != null)
            bombObject.SetActive(false);

        StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro()
    {
        // 1. Le personnage entre
        animator.Play(walkInAnim);
        yield return MoveTo(placePoint.position);

        // 2. Il s'arrête et place la bombe
        animator.Play(placeBombAnim);
        yield return new WaitForSeconds(0.5f); // Laisse le temps à l'anim de commencer

        // 3. Activation de la bombe
        if (bombObject != null)
        {
            bombObject.SetActive(true); // Active le GameObject contenant le script BombIntro
        }

        yield return new WaitForSeconds(0.5f); // Laisse l'anim de pose se finir

        // 4. Il repart
        animator.Play(walkOutAnim);
        yield return MoveTo(exitPoint.position);

        // 5. Attendre qu’il sorte de la caméra (pour la mise en scène)
        yield return WaitUntilOffscreen();

        // 6. Lance la bombe (séquence explosion)
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
