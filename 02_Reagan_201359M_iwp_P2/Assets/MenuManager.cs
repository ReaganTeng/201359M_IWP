using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void ReturnToHubWorld()
    {
        PlayerPrefs.SetInt("RoundsCompleted",
            PlayerPrefs.GetInt("RoundsCompleted") + 1);
        int roundsCompleted = PlayerPrefs.GetInt("RoundsCompleted");

        if (roundsCompleted >= 2)
        {
            PlayerPrefs.SetInt("RoundsCompleted", 0);
            PlayerPrefs.SetInt("DaysLeft",
            PlayerPrefs.GetInt("DaysLeft") - 1);
        }

        //LOAD BACK TO HUBWORLD;
        SceneManager.LoadScene("HubWorld");
    }


    public void RestartGame()
    {
        //LOAD BACK TO HUBWORLD;
        //SceneManager.LoadScene("HubWorld");
    }
}
