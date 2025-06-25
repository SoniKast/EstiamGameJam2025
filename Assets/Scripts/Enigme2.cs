using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enigme2 : MonoBehaviour
{
    [System.Serializable]
    public class CablePoint
    {
        public string id; // exemple : "red", "blue"
        public Button startButton;
        public Button endButton;
        public Image filConnecte;
        public bool isConnected = false;
    }

    public List<CablePoint> cables;

    private string selectedStartId = null;

    void Start()
    {
        foreach (var cable in cables)
        {
            cable.startButton.onClick.AddListener(() => OnStartSelected(cable.id));
            cable.endButton.onClick.AddListener(() => OnEndSelected(cable.id));
        }
    }

    void OnStartSelected(string id)
    {
        selectedStartId = id;
        Debug.Log("Départ sélectionné : " + id);
    }

    void OnEndSelected(string id)
    {
        if (selectedStartId == null) return;

        if (id == selectedStartId)
        {
            Debug.Log("Connexion réussie : " + id);
            var cable = cables.Find(c => c.id == id);
            cable.isConnected = true;
            cable.startButton.interactable = false;
            cable.endButton.interactable = false;
            cable.filConnecte.gameObject.SetActive(true);


            CheckVictory();
        }
        else
        {
            Debug.Log("Mauvaise connexion : " + selectedStartId + " -> " + id);
            ResetAll();
        }

        selectedStartId = null;
    }

    void CheckVictory()
    {
        foreach (var cable in cables)
        {
            if (!cable.isConnected)
                return;
        }

        Debug.Log("Tous les câbles sont connectés !");
        FindObjectOfType<GameUIManager>().OnEnigme2Completed();
    }

    void ResetAll()
    {
        selectedStartId = null;

        foreach (var cable in cables)
        {
            cable.isConnected = false;
            cable.startButton.interactable = true;
            cable.endButton.interactable = true;
            cable.filConnecte.gameObject.SetActive(false);
        }
    }
}
