using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public GameObject PNL_Settings;
    public GameObject PNL_Credits;


    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void OpenSettings()
    {
        PNL_Settings.SetActive(true);
    }

    public void OpenCredits()
    {
        PNL_Credits.SetActive(true);
    }

    public void Quit()
    {
    #if (UNITY_EDITOR || DEVELOPMENT_BUILD)
        Debug.Log(this.name+" : "+this.GetType()+" : "+System.Reflection.MethodBase.GetCurrentMethod().Name); 
    #endif
    #if (UNITY_EDITOR)
        UnityEditor.EditorApplication.isPlaying = false;
    #elif (UNITY_STANDALONE) 
        Application.Quit();
    #elif (UNITY_WEBGL)
        Application.OpenURL("https://diogenesyazan.itch.io/");
    #endif
    }

    public void BackButton()
    {
        PNL_Settings.SetActive(false);
        PNL_Credits.SetActive(false);
    }
}
