using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject panelMainMenu;
    public GameObject panelEnigme1;
    public GameObject panelEnigme2;
    public GameObject panelEnigme3;

    [Header("Buttons")]
    public Button enigme1Button;
    public Button enigme2Button;
    public Button enigme3Button;

    private bool enigme1Done = false;
    private bool enigme2Done = false;
    private bool enigme3Done = false;

    void Start()
    {
        ShowPanel(panelMainMenu);
        UpdateButtons();
    }

    void UpdateButtons()
    {
        enigme1Button.interactable = !enigme1Done;
        enigme2Button.interactable = !enigme2Done;
        enigme3Button.interactable = !enigme3Done;
    }

    void ShowPanel(GameObject panelToShow)
    {
        panelMainMenu.SetActive(false);
        panelEnigme1.SetActive(false);
        panelEnigme2.SetActive(false);
        panelEnigme3.SetActive(false);

        panelToShow.SetActive(true);
    }

    public void OnClick_Enigme1() => ShowPanel(panelEnigme1);
    public void OnClick_Enigme2() => ShowPanel(panelEnigme2);
    public void OnClick_Enigme3() => ShowPanel(panelEnigme3);

    public void OnEnigme1Completed()
    {
        enigme1Done = true;
        ShowPanel(panelMainMenu);
        UpdateButtons();
    }

    public void OnEnigme2Completed()
    {
        enigme2Done = true;
        ShowPanel(panelMainMenu);
        UpdateButtons();
    }

    public void OnEnigme3Completed()
    {
        enigme3Done = true;
        ShowPanel(panelMainMenu);
        UpdateButtons();
    }
}
