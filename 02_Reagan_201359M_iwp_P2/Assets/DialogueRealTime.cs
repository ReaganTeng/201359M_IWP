using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

[Serializable]
public class DialogueRealTime : MonoBehaviour
{
    [Serializable]
   
    public enum DialogueGiver
    {
        TUTORIAL,
        SHOPKEEPER,
        QUEST_GIVER,
    }

    public DialogueGiver dialogueGiver;

    [Serializable]
    public class DialogueOption
    {
        public string optionText;
        public Action onOptionSelected;
    }

    [Serializable]
    public class Sentence
    {
        public string text;
        public List<DialogueOption> options;
    }

    public List<Sentence> sentences;

    public void Initialize()
    {
        //HARD CODE THE POTENTIAL FUNCTIONS HERE BASED ON WHAT DIALOGUEGIVE IT IS
        switch (dialogueGiver)
        {
            case DialogueGiver.SHOPKEEPER:
                // Assuming you have a valid reference to the DialoguePanel GameObject
                // YES OPTION
                sentences[0].options[0].onOptionSelected += EnableShopPanel;
                // NO OPTION
                sentences[0].options[1].onOptionSelected += CloseDialogue;
                break;

            default:
                break;
        }
    }

    public void InitializeQuestGiver(ref QuestManager QM, QuestTrigger QT)
    {

        //HARD CODE THE POTENTIAL FUNCTIONS HERE BASED ON WHAT DIALOGUEGIVE IT IS
        switch (dialogueGiver)
        {
            case DialogueGiver.QUEST_GIVER:
                // Create a local variable to capture the correct reference of givenQuest
                //QuestTrigger localQuestTrigger = QT;
                //Debug.Log($"LOC BOOL IS {localGivenQuest}");
                QuestManager localQM = QM;
                // YES OPTION
                sentences[0].options[0].onOptionSelected += () => {
                    //Debug.Log($"GIVEN QUEST IS {localGivenQuest}");
                    AcceptQuest(ref localQM, QT);
                    //Debug.Log($"GIVEN QUEST IS {localGivenQuest}");
                };
                // NO OPTION
                sentences[0].options[1].onOptionSelected += DeclineQuest;
                //QT.givenQuest = true;
                break;
            default:
                break;
        }
    }



    void EnableShopPanel()
    {

        Debug.Log("EnableShopPanel called");
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameMGT");
        CanvasGroup shopPanel = null;

        Scene currentScene = SceneManager.GetActiveScene();
        switch (currentScene.name)
        {
            case "HubWorld":
                {
                    HubWorldMenuManager menuManager = gameManager.GetComponent<HubWorldMenuManager>();
                    shopPanel = menuManager.ShopPanel.GetComponent<CanvasGroup>();
                    break;
                }
            case "GameScene":
                {
                    MenuManager menuManager = gameManager.GetComponent<MenuManager>();
                    shopPanel = menuManager.ShopPanel.GetComponent<CanvasGroup>();
                    break;
                }
            default:
                break;
        }
        shopPanel.interactable = true;
        shopPanel.blocksRaycasts = true;
        shopPanel.alpha = 1;

        //close dialoguuepanel

        gameManager.GetComponent<DialogueManager>().CloseDialogue();
        //gameManager.GetComponent<DialogueManager>().dialoguePanel.GetComponent<CanvasGroup>().interactable = false;
        //gameManager.GetComponent<DialogueManager>().dialoguePanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
        //gameManager.GetComponent<DialogueManager>().dialoguePanel.GetComponent<CanvasGroup>().alpha = 0;
    }


    void CloseDialogue()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameMGT");
        gameManager.GetComponent<DialogueManager>().CloseDialogue();
        //gameManager.GetComponent<DialogueManager>().dialoguePanel.GetComponent<CanvasGroup>().interactable = false;
        //gameManager.GetComponent<DialogueManager>().dialoguePanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
        //gameManager.GetComponent<DialogueManager>().dialoguePanel.GetComponent<CanvasGroup>().alpha = 0;
    }


    public void AcceptQuest(ref QuestManager QM, QuestTrigger QT)

    {
        //for (int i = 0; i < 10; i++)
        {
            //int localID = QT.questGiverid;
            //Quest foundQuest = QM.quests.Find(template => template.hiddenVariables.questGiverID == localID);
            //if (foundQuest != null)
            //{
            //Debug.Log("TRIGGERED");
            QT.requiredCount = 1;
            //QT.requiredCount = Random.Range(1, 11);
            //AddQuest(string questName, string description, int requiredCount);
            QM.AddQuest(QT.questName, QT.QuestDes, QT.requiredCount, QT.questGiverid);
            //Debug.Log($"GIVEN QUEST IS {givenQuest}");
            //}
        }
        QT.givenQuest = true;


        CloseDialogue();
    }


    void DeclineQuest()
    {
        CloseDialogue();
    }


   
}
