using UnityEngine;
using UnityEngine.UI;

public class Enigme3 : MonoBehaviour
{
    public Button[] buttons; // Boutons de 0 � 9 dans l'inspecteur, dans l'ordre
    private int currentIndex = 0;

    void Start()
    {
        // Ajout des listeners aux boutons
        for (int i = 0; i < buttons.Length; i++)
        {
            int value = i; // Capture la valeur du bouton
            buttons[i].onClick.AddListener(() => OnNumberClicked(value));
        }

        ResetButtons();
    }

    void OnNumberClicked(int number)
    {
        if (number == currentIndex)
        {
            buttons[number].interactable = false;
            currentIndex++;

            if (currentIndex >= buttons.Length)
            {
                Debug.Log("�nigme 3 r�ussie !");
                FindObjectOfType<GameUIManager>().OnEnigme3Completed();
            }
        }
        else
        {
            Debug.Log("Mauvais chiffre cliqu� : " + number + "-> Reset de la progression.");
            ResetButtons();
        }
    }

    void ResetButtons()
    {
        currentIndex = 0;
        // R�activer tous les boutons
        foreach (Button btn in buttons)
        {
            btn.interactable = true;
        }
    }
}
