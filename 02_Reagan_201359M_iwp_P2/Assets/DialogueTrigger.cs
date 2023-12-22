using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    [HideInInspector]
    public GameObject DM;

    void Awake()
    {
        DM = GameObject.FindGameObjectWithTag("GameMGT"); 
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("TRIGGERING DIALOGUE");
            TriggerDialogue();
        }
    }

    void TriggerDialogue()
    {
        DM.GetComponent<DialogueManager>().StartDialogue(dialogue);
    }
}
