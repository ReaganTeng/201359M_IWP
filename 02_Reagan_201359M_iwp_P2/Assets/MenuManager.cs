using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void ReturnToHubWorld()
    {


        //LOAD BACK TO HUBWORLD;
        SceneManager.LoadScene("HubWorld");
    }


    public void ReturnToHubWorldFromGameOver()
    {

        SceneManager.LoadScene("HubWorld");

    }


    public void RestartGame()
    {
        //LOAD BACK TO HUBWORLD;
        //SceneManager.LoadScene("HubWorld");
    }
}
