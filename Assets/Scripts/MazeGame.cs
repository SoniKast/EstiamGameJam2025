using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;


public class MazeGame : MonoBehaviour
{
    public static MazeGame Instance;

    public bool isPlaying = false;
    public Button startButton;
    public Button finishButton;

    public TextMeshProUGUI resultText;

    public AudioSource right;
    public AudioSource wrong;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        startButton.onClick.AddListener(StartGame);
        finishButton.onClick.AddListener(CheckVictory);
    }

    void StartGame()
    {
        isPlaying = true;
        resultText.color = new Color(173, 0, 0, 255);
        resultText.text = "...";
        Debug.Log("Départ du labyrinthe !");
    }

    void CheckVictory()
    {
        if (isPlaying)
        {
            isPlaying = false;
            Debug.Log("Bravo, vous avez gagné !");
            right.Play();
            resultText.color = new Color(0, 173, 0, 255);
            resultText.text = "Victoire !";
        }
    }

    public void Lose()
    {
        isPlaying = false;
        Debug.Log("Perdu ! Vous avez touché un mur.");
        wrong.Play();
        resultText.color = new Color(173, 0, 0, 255);
        resultText.text = "Perdu. Recommencez";    
    }


}
