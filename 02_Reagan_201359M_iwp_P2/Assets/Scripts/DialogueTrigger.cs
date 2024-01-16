using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class DialogueTrigger : Interactables
{
    [HideInInspector]
    public DialogueRealTime dialogue;

    [HideInInspector]
    public DialogueManager DM;


    public override void Awake()
    {
        base.Awake();
        dialogue = GetComponent<DialogueRealTime>();


        DM = GameObject.FindGameObjectWithTag("GameMGT").GetComponent<DialogueManager>();
        dialogue.Initialize();
    }

    public override void Update()
    {
        
    }

    public override void Interact()
    {
        Debug.Log("IMTERACT TRIGGER");
        //base.Interact();
        TriggerDialogue();
    }

    public virtual void TriggerDialogue()
    {
        DM.StartDialogue(dialogue);
    }
}
