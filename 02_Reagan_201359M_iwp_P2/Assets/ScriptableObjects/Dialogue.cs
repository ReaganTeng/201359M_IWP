using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{
    public enum DialogueGiver
    {
        TUTORIAL,
        SHOPKEEPER,
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

    void EnableShopPanel()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameMGT");
        MenuManager menuManager = gameManager.GetComponent<MenuManager>();
        CanvasGroup shopPanel = menuManager.ShopPanel.GetComponent<CanvasGroup>();
        shopPanel.interactable = true;
        shopPanel.blocksRaycasts = true;
        shopPanel.alpha = 1;

        // Replace this with your logic to close the dialogue panel
        gameManager.GetComponent<DialogueManager>().CloseDialogue();
    }


    void CloseDialogue()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameMGT");
        // Replace this with your logic to close the dialogue panel
        gameManager.GetComponent<DialogueManager>().CloseDialogue();
    }
}