using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{
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
                //YES OPTION
                sentences[0].options[0].onOptionSelected += EnableShopPanel;
                //NO OPTION
                sentences[0].options[1].onOptionSelected += CloseDialogue;
                break;
            
            default:
                break;
        }
    }


    //public void InitializeQuestGiver(ref QuestManager QM, string questName, string QuestDes, int requiredCount, int questGiverid, ref bool givenQuest, GameObject GO)
    //{
    //    //HARD CODE THE POTENTIAL FUNCTIONS HERE BASED ON WHAT DIALOGUEGIVE IT IS
    //    switch (dialogueGiver)
    //    {
    //        case DialogueGiver.QUEST_GIVER:
    //            // Create a local variable to capture the correct reference of givenQuest
    //            bool localGivenQuest = givenQuest;
    //            QuestManager localQM = QM;

    //            if (!givenQuest)
    //            {
    //                // YES OPTION
    //                sentences[0].options[0].onOptionSelected += () => AcceptQuest(ref localQM, questName, QuestDes,
    //                requiredCount, questGiverid, ref localGivenQuest, GO);
    //                // NO OPTION
    //                sentences[0].options[1].onOptionSelected += DeclineQuest;
    //            }
    //            else
    //            {
    //                Quest quest = localQM.quests.Find(template
    //                    => template.hiddenVariables.questGiverID == questGiverid);
    //                if (quest.hiddenVariables.isCompleted)
    //                {
    //                    Debug.Log("QUEST COMPLETED");
    //                    int idx = QM.quests.IndexOf(quest);
    //                    GameObject questUI = QM.questUIContent.GetComponent<RectTransform>().GetChild(idx).gameObject;
    //                    Destroy(questUI);
    //                    QM.quests.Remove(quest);
    //                    Destroy(GO);
    //                }
    //                else
    //                {
    //                    Debug.Log("QUEST NOT COMPLETED");
    //                }
    //            }
    //            break;
    //        default:
    //            break;
    //    }
    //}




    void EnableShopPanel()
    {
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


    public void AcceptQuest(ref QuestManager QM, string questName, string QuestDes, int requiredCount, int questGiverid, ref bool givenQuest,
        GameObject GO)
    {
        // Create a local variable to capture the current value of givenQuest
        //bool localGivenQuest = givenQuest;

        //CHECK IF QUEST IS ALREADY GIVEN
        if (!givenQuest)
        {
            Quest foundQuest = QM.quests.Find(template => template.hiddenVariables.questGiverID == questGiverid);
            if (foundQuest == null)
            {
                //Debug.Log("TRIGGERED");
                requiredCount = 3;
                //AddQuest(string questName, string description, int requiredCount);
                QM.AddQuest(questName, QuestDes, requiredCount, questGiverid);
                //localGivenQuest = true;
                givenQuest = true;
            }
            //Debug.Log("TRIGGERED");
        }
        //else
        //{
        //    //CHECK IF QUEST IS ALD COMPLETED
        //    Quest quest = QM.quests.Find(template
        //        => template.hiddenVariables.questGiverID == questGiverid);

        //    if (quest.hiddenVariables.isCompleted)
        //    {
        //        Debug.Log("QUEST COMPLETED");
        //        int idx = QM.quests.IndexOf(quest);
        //        GameObject questUI = QM.questUIContent.GetComponent<RectTransform>().GetChild(idx).gameObject;
        //        Destroy(questUI);
        //        QM.quests.Remove(quest);
        //        Destroy(GO);
        //    }
        //    else
        //    {
        //        Debug.Log("QUEST NOT COMPLETED");
        //    }
        //}

        CloseDialogue();
    }

    void DeclineQuest()
    {
        CloseDialogue();
    }
}