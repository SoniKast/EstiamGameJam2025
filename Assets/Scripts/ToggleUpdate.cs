using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class ToggleUpdate : MonoBehaviour
{
    public Sprite on;
    public Sprite off;

    public Image image;

    void Update()
    {
        if (GetComponent<Toggle>().isOn)
        {
            image.GetComponent<Image>().sprite = on;
        }
        else
        {
            image.GetComponent<Image>().sprite = off;
        }
                
    }
}
