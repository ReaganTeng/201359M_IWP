using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    [HideInInspector]
    public GameObject instructions;
    [HideInInspector] public GameObject QuestPanel;
    [HideInInspector]
    public GameObject powerupNotificationPanel;
    [HideInInspector]
    public GameObject dialoguePanel;
    [HideInInspector]
    public GameObject ShopPanel;
    [HideInInspector]
    public GameObject GamePlayPanel;
    void Awake()
    {
        dialoguePanel = GameObject.FindGameObjectWithTag("DialoguePanel");
        powerupNotificationPanel = GameObject.FindGameObjectWithTag("PowerUpNotification");
        instructions = GameObject.FindGameObjectWithTag("Instructions");
        QuestPanel = GameObject.FindGameObjectWithTag("QuestPanel");
        ShopPanel = GameObject.FindGameObjectWithTag("ShopPanel");
        GamePlayPanel = GameObject.FindGameObjectWithTag("GamePlayPanel");


        togglePanel(dialoguePanel);
        togglePanel(powerupNotificationPanel);
        togglePanel(QuestPanel);
        togglePanel(ShopPanel);

        //togglePanel(instructions);

        //GamePlayPanel.GetComponent<CanvasGroup>().interactable = true;
        //GamePlayPanel.GetComponent<CanvasGroup>().alpha = 1;
        //GamePlayPanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }





    public void togglePanel(GameObject panel)
    {
        Debug.Log("TOGGLING PANEL");
        CanvasGroup panelCG = panel.GetComponent<CanvasGroup>();
        panelCG.interactable = !panelCG.interactable;
        panelCG.blocksRaycasts = !panelCG.blocksRaycasts;
        if(panelCG.interactable
           && panelCG.blocksRaycasts)
        {
            panelCG.alpha = 1;
        }
        else
        {
            panelCG.alpha = 0;
        }

        
    }


    public void togglePanel_CloseButton()
    {
        CanvasGroup panelCG = GetComponentInParent<CanvasGroup>();
        panelCG.interactable = !panelCG.interactable;
        panelCG.blocksRaycasts = !panelCG.blocksRaycasts;
        if (panelCG.interactable
           && panelCG.blocksRaycasts)
        {
            panelCG.alpha = 1;
        }
        else
        {
            panelCG.alpha = 0;
        }

        //TOGGLE GAMEPLAY PANEL
        togglePanel(GamePlayPanel);
    }



    public void Update()
    {
        //togglePanel(dialoguePanel);
        //togglePanel(powerupNotificationPanel);
        //togglePanel(instructions);
        //togglePanel(QuestPanel);


        if (!dialoguePanel.GetComponent<CanvasGroup>().interactable
        && !powerupNotificationPanel.GetComponent<CanvasGroup>().interactable
        && !QuestPanel.GetComponent<CanvasGroup>().interactable
        && !ShopPanel.GetComponent<CanvasGroup>().interactable)
        {
            if (!GamePlayPanel.GetComponent<CanvasGroup>().interactable)
            {
                GamePlayPanel.GetComponent<CanvasGroup>().interactable = true;
                GamePlayPanel.GetComponent<CanvasGroup>().alpha = 1;
                GamePlayPanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
            
            Character[] charactersInScene = FindObjectsOfType<Character>();
            foreach (Character character in charactersInScene)
            {
                character.disabled = false;
            }
        }
        else
        {


            if (GamePlayPanel.GetComponent<CanvasGroup>().interactable)
            {
                GamePlayPanel.GetComponent<CanvasGroup>().interactable = false;
                GamePlayPanel.GetComponent<CanvasGroup>().alpha = 0;
                GamePlayPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
            }



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


    //public void ToggleQuestPanel()
    //{
    //    QuestPanel.SetActive(!QuestPanel.activeSelf);
    //    if(QuestPanel.activeSelf )
    //    {
    //        //newQuestNot.SetActive(false);
    //        //questCompletedNot.SetActive(false);
    //    }
    //}
}
