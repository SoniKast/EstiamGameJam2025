using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject canvasCommencer;
    public GameObject canvasChoix;

    public string niv1;
    public string niv2;
    public string niv3;

    void Start()
    {
        // Masquer le canvas des niveaux au démarrage
        if (canvasChoix != null)
            canvasChoix.SetActive(false);
    }
    public void ChoixMenu()
    {
        // desactiver le canvas du début
        if (canvasCommencer != null)
            canvasCommencer.SetActive(false);

        // activer le canvas pour choisir un niveau
        if (canvasChoix != null)
            canvasChoix.SetActive(true);
    }

    public void Niveau1()
    {
        SceneManager.LoadScene(niv1);
    }
    public void Niveau2()
    {
        SceneManager.LoadScene(niv2);
    }
    public void Niveau3()
    {
        SceneManager.LoadScene(niv3);
    }
    public void Retour()
    {
        // desactiver le canvas pour choisir un niveau
        if (canvasChoix != null)
            canvasChoix.SetActive(false);

        // activer le canvas du début
        if (canvasCommencer != null)
            canvasCommencer.SetActive(true);
    }
    public void Quitter()
    {
        Application.Quit();
    }
}
