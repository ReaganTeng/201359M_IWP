using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{
    public enum DialogueGiver
    {
        TUTORIAL,
        SHOPKEEPER,
    }

    public DialogueGiver dialogueGiver;

    [System.Serializable]
    public class DialogueOption
    {
        public string optionText;
        public System.Action onOptionSelected;
    }

    [System.Serializable]
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
                sentences[0].options[1].onOptionSelected += CloseDialogue;
                break;
            default:
                break;
        }
    }

    void EnableShopPanel()
    {
        Debug.Log("ENABLE");
        // Replace this with your logic to close the dialogue panel
        GameObject.FindGameObjectWithTag("GameMGT").GetComponent<DialogueManager>().CloseDialogue();
        //GameObject.FindGameObjectWithTag("ShopPanel").GetComponent<CanvasGroup>().enabled = true;// SetActive(true);


        GameObject.FindGameObjectWithTag("ShopPanel").GetComponent<CanvasGroup>().interactable 
            = true;
        GameObject.FindGameObjectWithTag("ShopPanel").GetComponent<CanvasGroup>().alpha = 1;
    }


    void CloseDialogue()
    {
        Debug.Log("DISABLE");
        // Replace this with your logic to close the dialogue panel
        GameObject.FindGameObjectWithTag("GameMGT").GetComponent<DialogueManager>().CloseDialogue();
        //GameObject.FindGameObjectWithTag("ShopPanel").SetActive(false);
    }
}