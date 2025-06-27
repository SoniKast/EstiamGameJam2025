using UnityEngine;
using UnityEngine.UI;

public class Enigme3 : MonoBehaviour
{
    public Button[] buttons; // Boutons de 0 à 9 dans l'inspecteur, dans l'ordre
    private int currentIndex = 0;

    public AudioSource right;
    public AudioSource wrong;

    void Start()
    {
        // Ajout des listeners aux boutons
        for (int i = 0; i < buttons.Length; i++)
        {
            int value = i; // Capture la valeur du bouton
            buttons[i].onClick.AddListener(() => OnNumberClicked(value));
        }

        //ResetButtons();
    }

    void OnNumberClicked(int number)
    {
        if (number == currentIndex)
        {
            buttons[number].interactable = false;
            currentIndex++;

            if (currentIndex >= buttons.Length)
            {
                Debug.Log("Énigme 3 réussie !");
                right.Play();
                FindObjectOfType<GameUIManager>().OnEnigme3Completed();
            }
        }
        else
        {
            Debug.Log("Mauvais chiffre cliqué : " + number + "-> Reset de la progression.");
            ResetButtons();
        }
    }

    void ResetButtons()
    {
        currentIndex = 0;

        wrong.Play();

        foreach (Button btn in buttons)
        {
            btn.interactable = true;
        }
    }
}
