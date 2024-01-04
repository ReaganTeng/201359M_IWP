using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class DialogueTrigger : Interactables
{
    public Dialogue dialogue;

    [HideInInspector]
    public DialogueManager DM;

    public override void Awake()
    {
        base.Awake();
        DM = GameObject.FindGameObjectWithTag("GameMGT").GetComponent<DialogueManager>();
        dialogue.Initialize();
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) 
            && textPrompt.enabled)
        {
            TriggerDialogue();
        }
    }

    void TriggerDialogue()
    {
        DM.StartDialogue(dialogue);
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
    }

    public override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);
    }
}
