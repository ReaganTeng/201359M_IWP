using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    [HideInInspector]
    public GameObject instructions;
    [HideInInspector] public GameObject QuestPanel;

    public GameObject questCompletedNot;
    public GameObject newQuestNot;

    public void Awake()
    {
        instructions = GameObject.FindGameObjectWithTag("Instructions");
        if (instructions != null)
        {
            Time.timeScale = 0;
        }


        questCompletedNot.SetActive(false);
        newQuestNot.SetActive(false);

        QuestPanel = GameObject.FindGameObjectWithTag("QuestPanel");
        QuestPanel.SetActive(false);
    }


    public void Update()
    {
        
    }

    public void toggleInstruction()
    {
        if (instructions != null)
        {
            instructions.SetActive(!instructions.activeSelf);
            if (Time.timeScale > 0)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }


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


    public void ToggleQuestPanel()
    {
        QuestPanel.SetActive(!QuestPanel.activeSelf);
        if(QuestPanel.activeSelf )
        {
            newQuestNot.SetActive(false);
            questCompletedNot.SetActive(false);
        }
    }
}
