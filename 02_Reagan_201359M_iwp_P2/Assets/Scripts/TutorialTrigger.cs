using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class TutorialTrigger : MonoBehaviour
{
    public TutorialSystem tutorialSystem;


    //movementdialogue
    //attackdialogue
    //gemdialogue
    public List<Dialogue> tutorialDialogues;

    [HideInInspector]
    public GameObject DM;

    public void Awake()
    {
        //base.Awake();
        DM = GameObject.FindGameObjectWithTag("GameMGT");

        foreach (Dialogue dialogue in tutorialDialogues)
        {
            dialogue.Initialize();
        }
    }

    void Update()
    {
        //if ()
        //{
        //    TriggerDialogue();
        //}
    }

    void TriggerDialogue(Dialogue d, ref bool boolToSet)
    {
        if (!boolToSet)
        {
            DM.GetComponent<DialogueManager>().StartDialogue(d);
            boolToSet = !boolToSet;
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        switch(other.tag)
        {
            case "Player":
                TriggerDialogue(tutorialDialogues[0], ref tutorialSystem.MovementTutorialCompleted);
                break;
            case "Item":
                TriggerDialogue(tutorialDialogues[1], ref tutorialSystem.GemTutorialCompleted);
                break;
            case "Enemy":
                TriggerDialogue(tutorialDialogues[2], ref tutorialSystem.AttackTutorialCompleted);
                break;
            default:
                break;
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {

    }
}
