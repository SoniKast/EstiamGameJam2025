using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enigme1 : MonoBehaviour
{
    [System.Serializable]
    public class Interrupteur
    {
        public Toggle toggle;
        public Image diodeImage;
        public Color onColor;
        public Color offColor;
    }

    public Interrupteur[] interrupteurs;

    public GameObject panel; // pour désactiver le panel quand c’est terminé
    public GameObject menu; // activer le panel du menu quand c'est terminé

    void Update()
    {
        foreach (var i in interrupteurs)
        {
            // Met à jour la diode selon l'état de l'interrupteur
            i.diodeImage.color = i.toggle.isOn ? i.onColor : i.offColor;
        }

        if (AllOn())
        {
            Debug.Log("Tous les interrupteurs sont activés !");
            panel.SetActive(false); // Ferme le mini-jeu
            FindObjectOfType<GameUIManager>().OnEnigme1Completed();
            // menu.SetActive(true); // Ouvre le menu
            // Tu peux aussi envoyer un événement ou appeler une fonction ici
        }
    }

    bool AllOn()
    {
        foreach (var i in interrupteurs)
        {
            if (!i.toggle.isOn) return false;
        }
        return true;
    }

}
