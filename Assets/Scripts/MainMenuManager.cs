using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{

    public Sprite filsDone;
    public Sprite filsUndone;
    public Sprite switchDone;
    public Sprite switchUndone;
    public Button Fils;
    public Button Switch;

    void Update()
    {
        if (Fils.GetComponent<Button>().interactable)
        {
            Fils.GetComponent<Image>().sprite = filsUndone;
        }
        else
        {
            Fils.GetComponent<Image>().sprite = filsDone;
        }

        if (Switch.GetComponent<Button>().interactable)
        {
            Switch.GetComponent<Image>().sprite = switchUndone;
        }
        else
        {
            Switch.GetComponent<Image>().sprite = switchDone;
        }
    }
}
