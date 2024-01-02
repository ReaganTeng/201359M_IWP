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

    [HideInInspector]
    public GameObject powerupNotificationPanel;
    CanvasGroup powerUpCanvasGroup;
    //powerUpCanvasGroup.alpha = 0;
    //powerUpCanvasGroup.interactable = false;
    [HideInInspector]
    public GameObject dialoguePanel;


    public void Awake()
    {
        dialoguePanel = GameObject.FindGameObjectWithTag("DialoguePanel");

        powerupNotificationPanel = GameObject.FindGameObjectWithTag("PowerUpNotification");
        powerUpCanvasGroup = powerupNotificationPanel.GetComponent<CanvasGroup>();


        instructions = GameObject.FindGameObjectWithTag("Instructions");
        if (instructions != null)
        {
            Time.timeScale = 0;
        }
        questCompletedNot.SetActive(false);
        newQuestNot.SetActive(false);

        QuestPanel = GameObject.FindGameObjectWithTag("QuestPanel");
        QuestPanel.SetActive(false);
        
        //close the dialogue panel
        //GetComponent<DialogueManager>().CloseDialogue();
    }


    public void Update()
    {
        if(!powerUpCanvasGroup.interactable
        && !dialoguePanel.activeSelf
        && !QuestPanel.activeSelf)
        {
            Debug.Log("UNPAUSE MODE");
            Character[] charactersInScene = FindObjectsOfType<Character>();
            foreach (Character character in charactersInScene)
            {
                character.disabled = false;
            }
        }
        else
        {
            Debug.Log("PAUSE MODE");
            Character[] charactersInScene = FindObjectsOfType<Character>();
            foreach (Character character in charactersInScene)
            {
                character.disabled = true;
            }
        }

        //FREEZE ALL GAMEOBJECTS
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
